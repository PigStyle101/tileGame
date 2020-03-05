local mod = 'Core'
local title = 'Militia'
local type = 'Unit'
local description = 'Weak unit, but cheap'
local pixelsPerUnit = 650
local IdleAnimations = true
local AtackAnimations = true
local MoveAnimations = true
local HurtAnimations = true
local DiedAnimations = true
local IdleAnimationSpeed = 0.02
local AttackAnimationSpeed = 0.02
local MoveAnimationSpeed = 0.02
local HurtAnimationSpeed = 0.02
local DiedAnimationSpeed = 0.02
local MoveAnimationOffset = 20
local Attack = 2
local Defence = 0
local AttackRange = 1
local Health = 4
local MovePoints = 6
local Cost  = 100
local ConversionSpeed = 4
local SightRange = 4
local CanConvert = true
local CanMoveAndAttack = true

function GetAllData()
Unit.GetInfo(mod,title,type,description,pixelsPerUnit,IdleAnimations,AtackAnimations,MoveAnimations,HurtAnimations,DiedAnimations,IdleAnimationSpeed,AttackAnimationSpeed,MoveAnimationSpeed,HurtAnimationSpeed,DiedAnimationSpeed,MoveAnimationOffset,Attack,Defence,AttackRange,Health,MovePoints,Cost,ConversionSpeed,SightRange,CanConvert,CanMoveAndAttack)
end