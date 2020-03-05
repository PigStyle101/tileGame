local mod = 'Core'
local title = 'Elemental'
local type = 'Hero'
local description = 'Elemental hero'
local pixelsPerUnit = 600
local MovePoints = 5
local SightRange = 4
local AttackRange = 1
local CanConvert = true
local CanMoveAndAttack = true
local BaseInetelligance = 1
local BaseStrenght = 3
local BaseDexterity = 2
local BaseCharisma = 0
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