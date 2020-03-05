local mod = 'Core'
local title = 'Archer'
local type = 'Unit'
local description = 'Ranged unit with decent movement but low defence'
local pixelsPerUnit = 600
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
local Defence = 4
local AttackRange = 3
local Health = 5
local MovePoints = 4
local Cost  = 200
local ConversionSpeed = 2
local SightRange = 6
local CanConvert = true
local CanMoveAndAttack = true

function GetAllData()
Unit.GetInfo(mod,title,type,description,pixelsPerUnit,IdleAnimations,AtackAnimations,MoveAnimations,HurtAnimations,DiedAnimations,IdleAnimationSpeed,AttackAnimationSpeed,MoveAnimationSpeed,HurtAnimationSpeed,DiedAnimationSpeed,MoveAnimationOffset,Attack,Defence,AttackRange,Health,MovePoints,Cost,ConversionSpeed,SightRange,CanConvert,CanMoveAndAttack)
end