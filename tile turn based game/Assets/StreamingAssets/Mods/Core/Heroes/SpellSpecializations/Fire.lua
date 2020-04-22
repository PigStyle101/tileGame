--Specialisation class? named Fire
local FireSpells = {}
local FireBall = {}
local Cauterize = {}
local HolySpells = {}
local Heal = {}
local Smite = {}

FireSpells.FireBall = FireBall
FireSpells.FireBall.name = 'FireBall'
FireSpells.FireBall.cost = 5
FireSpells.Cauterize = Cauterize
FireSpells.Cauterize.name = 'Caut'
FireSpells.Cauterize.cost =2

HolySpells.Heal = Heal
HolySpells.Heal.name = 'Heal'
HolySpells.Heal.cost = 2
HolySpells.Smite = Smite
HolySpells.Smite.name = 'Smite'
HolySpells.Smite.cost = 3

function CastSpell(name,mana)
  if name == FireSpells.Cauterize.name then
    if mana >= FireSpells.Cauterize.cost then
    print('Casting Caut')
    else
    print('Need more mana for caut')
    end
  elseif name == FireSpells.FireBall.name then
    if mana >= FireSpells.FireBall.cost then
    print('Casting Fire')
    else
    print('Need more mana for fire')
    end
  end
end

function GetSpells(spec)
  if spec == 'Fire' then
    for k,v in pairs(FireSpells) do print(v.name) end
  elseif spec == 'Holy' then
    for k,v in pairs(HolySpells) do print(v.name) end
  end
end

GetSpells('Fire')
--CastSpell('FireBall',2)
--Classes for hero classes (mage,warrior)

--k=1,2 v=Fire,Holy
--for k,v in pairs(Mage.SpellSpecs) do print(k,v) end
--for k,v in pairs(FireSpells) do print(v.name) end 
--use the second for to get names for spells.

 