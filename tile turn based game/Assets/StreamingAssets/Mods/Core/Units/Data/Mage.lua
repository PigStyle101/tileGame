local mod = 'Core'
local title = 'Mage'
local type = 'Unit'
local description = 'Cast spells over a longer range, but needs a turn to get set up'
local pixelsPerUnit = 700
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
local Attack = 5
local Defence = 10
local AttackRange = 4
local Health = 5
local MovePoints = 2
local Cost  = 300
local ConversionSpeed = 2
local SightRange = 2
local CanConvert = false
local CanMoveAndAttack = false

function GetAllData()
Unit.GetInfo(mod,title,type,description,pixelsPerUnit,IdleAnimations,AtackAnimations,MoveAnimations,HurtAnimations,DiedAnimations,IdleAnimationSpeed,AttackAnimationSpeed,MoveAnimationSpeed,HurtAnimationSpeed,DiedAnimationSpeed,MoveAnimationOffset,Attack,Defence,AttackRange,Health,MovePoints,Cost,ConversionSpeed,SightRange,CanConvert,CanMoveAndAttack)
end