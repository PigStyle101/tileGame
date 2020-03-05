local mod = 'Core'
local title = 'Undead'
local type = 'Hero'
local description = 'Undead hero'
local pixelsPerUnit = 600
local MovePoints = 5
local SightRange = 4
local AttackRange = 1
local CanConvert = true
local CanMoveAndAttack = true
local BaseInetelligance = 2
local BaseStrenght = 1
local BaseDexterity = 1
local BaseCharisma = 2
local IdleAnimation = true
local AttackAnimation = true
local MoveAnimation = true
local HurtAnimation = true
local DiedAnimation = true
local IdleAnimationSpeed = 0.02
local AttackAnimationSpeed = 0.02
local MoveAnimationSpeed = 0.02
local HurtAnimationSpeed = 0.02
local DiedAnimationSpeed = 0.02
local MoveAnimationOffset = 20

function GetAllData()
Hero.GetInfo(mod,title,type,description,pixelsPerUnit,MovePoints,SightRange,AttackRange,CanConvert,CanMoveAndAttack,BaseInetelligance,BaseStrenght,BaseDexterity,BaseCharisma,IdleAnimation,AttackAnimation,MoveAnimation,HurtAnimation,DiedAnimation,IdleAnimationSpeed,AttackAnimationSpeed,MoveAnimationSpeed,HurtAnimationSpeed,DiedAnimationSpeed,MoveAnimationOffset)
end