local mod = 'Core'
local title = 'Road'
local type = 'Terrain'
local description = 'A well worn path'
local pixelsPerUnit = 64
local DefenceBonus = 0
local Wheight = 1
local BlocksSight = false
local IdleAnimations = false
local Overlays = false
local Connectable = true
local Walkable = true

function GetAllData()
Terrain.GetInfo(mod,title,type,description,pixelsPerUnit,DefenceBonus,Wheight,BlocksSight,IdleAnimations,Overlays,Connectable,Walkable)
end