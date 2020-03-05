local mod = 'Core'
local title = 'Soldier'
local type = 'Unit'
local description = 'Tanky unit with good damage but low movement'
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
local Attack = 4
local Defence = 20
local AttackRange = 1
local Health = 10
local MovePoints = 4
local Cost  = 400
local ConversionSpeed = 2
local SightRange = 3
local CanConvert = true
local CanMoveAndAttack = true

function GetAllData()
Unit.GetInfo(mod,title,type,description,pixelsPerUnit,IdleAnimations,AtackAnimations,MoveAnimations,HurtAnimations,DiedAnimations,IdleAnimationSpeed,AttackAnimationSpeed,MoveAnimationSpeed,HurtAnimationSpeed,DiedAnimationSpeed,MoveAnimationOffset,Attack,Defence,AttackRange,Health,MovePoints,Cost,ConversionSpeed,SightRange,CanConvert,CanMoveAndAttack)
end