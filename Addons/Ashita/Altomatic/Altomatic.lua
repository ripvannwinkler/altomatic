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

-- ashita.register_event('incoming_text', function(mode, chat)
--   for k, v in pairs(onevent.events) do
--       if (chat:contains(v[1])) then
--           AshitaCore:GetChatManager():QueueCommand(v[2], 1);
--       end
--   end
--   return false;
-- end);

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
  end

  return true;
end

ashita.register_event('incoming_packet', HandleIncomingPacket)
ashita.register_event('command', HandleAddonCommand)
