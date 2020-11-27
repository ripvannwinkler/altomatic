_addon.name = 'Altomatic'
_addon.author = 'ripvannwinkler'
_addon.version = '1.0.0 Ashita'
_addon.description = 'Sends party buffs and cast completed info to the Altomatic tool'
_addon.commands = {'alto'}

require 'common'

local PACKET_CASTING_STATUS = 0x28
local PACKET_PARTY_BUFFS = 0x076
local PACKET_ZONE_BEGIN = 0xB
local PACKET_ZONE_END = 0xA

local zoning = false
local socket = require("socket")
local ip = "127.0.0.1"
local port = 19769

-- The chat log is only used to track critical debuffs on potential
-- alliance members, since they are not sent via packet like party
-- member debuffs are. Do not add anything here that cannot be removed
-- on alliance members (e.g., anything via Erase).
local chatBuffPatterns = {
  -- doom
  {'(%w+) is doomed.', 'add', 15},
  {'(%w+) is no longer doomed.', 'remove', 15},
  {'removes (%w+)\'s doom.', 'remove', 15},
  {'Cursna has no effect on (%w+).', 'remove', 15},
  -- curse
  {'(%w+) is cursed.', 'add', 9},
  {'(%w+) is no longer cursed.', 'remove', 9},
  {'removes (%w+)\'s curse.', 'remove', 9},
  {'Cursna has no effect on (%w+).', 'remove', 9},
  -- petrification
  {'(%w+) is petrified.', 'add', 7},
  {'(%w+) is no longer petrified.', 'remove', 7},
  {'removes (%w+)\'s petrification.', 'remove', 7},
  {'Stona has no effect on (%w+).', 'remove', 7},
  -- paralysis
  {'(%w+) is paralyzed.', 'add', 4},
  {'(%w+) is no longer paralyzed.', 'remove', 4},
  {'removes (%w+)\'s paralysis.', 'remove', 4},
  {'Paralyna has no effect on (%w+).', 'remove', 4},
  -- plague
  {'(%w+) is plagued.', 'add', 31},
  {'(%w+) is no longer plagued.', 'remove', 31},
  {'removes (%w+)\'s plague.', 'remove', 31},
  {'Viruna has no effect on (%w+).', 'remove', 31},
  -- disease
  {'(%w+) is diseased.', 'add', 8},
  {'(%w+) is no longer diseased.', 'remove', 8},
  {'removes (%w+)\'s disease.', 'remove', 8},  
  {'Viruna has no effect on (%w+).', 'remove', 8},
  -- sleep
  {'(%w+) is asleep.', 'add', 2},
  {'(%w+) is no longer asleep.', 'remove', 2},
  {'(%w+) recovers %d+ HP.', 'remove', 2},  
  -- silence
  {'(%w+) is silenced.', 'add', 6},
  {'(%w+) is no longer silenced.', 'remove', 6},
  {'removes (%w+)\'s silence.', 'remove', 6},
  {'Silena has no effect on (%w+).', 'remove', 6}
}

function CleanString(str)
  str = ParseAutoTranslate(str, true);
  str = str:gsub('[\r\n]+$','') -- trim end of string line breaks
  str = str:gsub('[' .. string.char(0x1E, 0x1F, 0x7F) .. '].', ''); -- strip color codes
  str = str:gsub(string.char(0xEF) .. '[' .. string.char(0x27) .. ']', '{'); -- strip auto tranlate begin
  str = str:gsub(string.char(0xEF) .. '[' .. string.char(0x28) .. ']', '}'); -- strip auto translate end
  str = str:gsub(string.char(0x07), '\n'); -- convert mid line breaks to real line breaks  
  return str;
end

function GetPlayerName()
  local player = GetPlayerEntity()
  if player ~= nil then
      return player.Name
  end
  return nil
end


function SendToAltomatic(message)
  pcall(function()
    local altos = socket.udp()
    altos:settimeout(1)
    altos:sendto('alto:' .. message, ip, port)
    altos:close()
  end)
end

function GetCharacterName(uid)
  return AshitaCore:GetDataManager():GetEntity():GetName(uid)
end

function ParseBuffsData(id, data)
  local uid = nil
  local buffs = {}
  local buffCount = 0
  local charName = nil
  local formatted = nil

  for k = 0, 4 do
    uid = struct.unpack('H', data, 8 + 1 + (k * 0x30))
    charName = uid == nil and nil or GetCharacterName(uid)
    -- build buffs status
    if charName ~= nil then
      buffs = {}
      buffCount = 0
      for i = 1, 32 do
        local buff = data:byte(k * 48 + 5 + 16 + i - 1) + 256 * (math.floor(data:byte(k * 48 + 5 + 8 + math.floor((i - 1) / 4)) / 4 ^ ((i - 1) % 4)) % 4)
        if buff > 0 and buff < 255 then
          table.insert(buffs, buff)
          buffCount = buffCount + 1
        end
      end
      formatted = 'buffs_' .. charName .. '_' .. table.concat(buffs, ',')
      SendToAltomatic(formatted)
    end
  end
end

function ConfirmAddonLoaded()
  SendToAltomatic("addon_loaded");
end

function HandleIncomingPacket(id, size, data)
  if id == PACKET_ZONE_BEGIN then zoning = true end
  if id == PACKET_ZONE_END and zoning then zoning = false end

  if not zoning then
    if id == PACKET_CASTING_STATUS then
      local actor = struct.unpack('I', data, 6);
      local category = ashita.bits.unpack_be(data, 82, 4);
      if actor == AshitaCore:GetDataManager():GetParty():GetMemberServerId(0) then
        if category == 4 then
          SendToAltomatic('casting_completed')
        elseif category == 8 then
          local temp = ashita.bits.unpack_be(data, 86, 16);
          if temp == 28787 then
            SendToAltomatic('casting_interrupted')
          elseif temp == 24931 then
            SendToAltomatic('casting_started')
          else 
            print('unknown casting status: ' .. temp)
          end
        elseif category == 6 then
          local effect = ashita.bits.unpack_be(data, 213, 17);
          if effect then
            SendToAltomatic("roll_"..effect)
          end
        end
      end
    elseif id == PACKET_PARTY_BUFFS then
      ParseBuffsData(id, data)
    end
  end

  return false
end

function HandleAddonCommand(command, ntype)
  local args = command:args();
  if (args[1]:lower() ~= '/alto') then
    return false;
  end

  if (#args >= 4 and args[2] == 'config') then
    ip = args[3]
    port = args[4]
    print('Altomatic configured; ip=' .. ip .. '; port=' .. port)
    ConfirmAddonLoaded()
  elseif #args >= 2 and args[2] == 'heartbeat' then
    SendToAltomatic("heartbeat");
  end

  return true;
end

function DetectBuffsFromChat(message)
  for _, p in pairs(chatBuffPatterns) do
    local player = string.match(message, p[1]);
    if player ~= nil then
      local add = p[2] == 'add';
      local buff = p[3];
      if add then
        print('buff add ' .. player .. ' ' .. buff)
        SendToAltomatic('buffAdd_'..player..'_'..buff)
      else
        print('buff remove ' .. player .. ' ' .. buff)
        SendToAltomatic('buffRemove_'..player..'_'..buff)
      end
    end
end
end

function HandleIncomingText(mode, message, modifiedmode, modifiedmessage, blocked)
  if message ~= nil then
      local cleanStr = CleanString(message);
      if cleanStr ~= nil and string.len(cleanStr)>0 then
        DetectBuffsFromChat(cleanStr);
      end
  end
  return false
end

ashita.register_event('incoming_text', HandleIncomingText);
ashita.register_event('incoming_packet', HandleIncomingPacket)
ashita.register_event('command', HandleAddonCommand)
