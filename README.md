# Spongia2020
Tu sa daju uploadovat vsetky veci a obrazky co spravite
Navod na doladovanie je [v kapitole konstanty](#konstanty)
### Prikazy
Ak si chceš projekt stiahnuť a naiinicializovat (pouzi len raz na zaciatku)
```bash
git clone https://github.com/Stanko2/Spongia2020.git
```
Ak nahrať tvoje zmeny použi
```bash
git commit -am "Sprava co som zmenil"
git push origin master
```
Ak aktualizovať - stiahnuť zmeny od ostatných
```bash
git pull origin master
```
### GUI
Tuto časť môžeš spraviť ak chceš mať Git integrovaný v Unity, ak si v pohode s príkazmy môžeš ignorovať
1. Vygeneruj a pridaj si do GitHub účtu ssh klúč
 - návod nájdeš [tu](https://docs.github.com/en/free-pro-team@latest/github/authenticating-to-github/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent)
2. Nainicializuj si projekt a spusti v unity (normálne otvoríš scénu)
```bash
git clone git@github.com:{tvoje meno v GitHube}/Spongia2020.git
```
3. V unity otvoríš Window > GitHub, ak to tam nie je ideš do Asset Store a stiahneš plugin GitHub
4. V rohu okna máš sign in - prihlásiš sa do GitHub účtu
5. V okne GitHub vies robiť operácie push, commit, pull, aj si pozriet históriu čo kto kedy menil a pridával

# Konstanty
Tu najdes zoznam vsetkych konstant, ktore mozes menit
## Item
Na vytvorenie itemu chod do precinku `ScriptableObjects/items` a skopiruj nejaky item, alebo vytvor novy pomocou praveho tlacitka
* `itemName` - meno itemu
* `description` - strucny popis, co ten item robi (zobrazi sa ked nad nim budes dlho mat mys v inventari)
* `spaceRequired` - kolko miesta zaberie tento item v inventari
* `icon` - ikonka itemu
* `type` - typ itemu 
  * Weapon - item sa bude dat equipnut ako zbran
   * `damage` - to je hadam jasne
   * `fireRate` - ako casto vieme zbran pouzivat (cooldown po pouziti)
   * `range` - ako daleko vieme dostrelit
  * Armor - da sa equipnut ako armor
   * `toughness` - ake percento damagu absorbuje armor (`damage co dostanes = povodny damage * (1-toughness)`)
  * InventoryUpgrade - da sa equipnut ako batoh
   * `backpackCapacity` - kapacita inventara, ak budem mat tento item equipnuty
  * Protection - da sa equipnut do slotu protection
   * `protectionLevel` - percento davky nakazenia, ktore ti tento item absorbuje (funguje rovnako ako armor)
  * other - tieto itemy su stackovatelne (mozes ich mat viac naraz), budu sluzit na craftenie a plnenie misii
## Enemy
Na vytvorenie noveho enemy chod do `Prefabs/Enemy` a tam nejakeho skopiruj
### EnemyMovement
Toto sluzi na hybanie a logiku enemakov. Kazdy enemy sa nejak hybe a ak uvidi hraca (hrac vojde do jeho triggeru), tak ho zacne nahanat a utocit nanho
* `speed` - rychlost enemy
* `followSpeed` - rychlost, ked prenasleduje hraca
* `attackRange` - vzdialenost, pri ktorej sa uz nepriblizuje k hracovi (moze utocit)
* `observeTime` - cas ktory sa obzera okolo ked pride na destinaciu
* `flashlight` - baterka, pripadne nejaky objekt, ktory bude otoceny podla toho kam sa enemy pozera
* `mode` - hovori ako si vybera destinaciu ked nevidi hraca
 * `idle` - vyberie si nahodny bod na ktory sa vie dostat a jeho maximalna vzdialenost od spawnu je `maxDefendRadius`
 * `defend` - stoji cely cas na spawne
 * `patrol` - do `waypoints` si zadas checkpointy na ktore sa bude chciet dostat a on ich potom dookola v poradi akom si ich zadal bude prechadzat
### EnemyAttack
Sluzi na attackovanie hraca
* `damage` - to je asi jasne
* `fireRate` - rovnako ako pri itemoch
* `attackMode` - sposob utocenia
 * `melee` - doda damage vsetkym objektom, ktore su znicitelne a su vzdialene najviac `meleeRange` od enemaka
 * `projectile` - enemy spawne projektil, ktory bude letiet ku hracovi. Projektil co sa ma spawnut sa nastavi v `projectile` (projectily este nie su hotove)
## Player
Tu najdes vsetky konstanty co nejak suvisia s hracom
### PlayerMovement
* `speed` - toto by mohlo byt jasne
* `flashlight` - rovnako ako pri enemy
### PlayerVitals
Tento skript sluzi na ovladanie toho ako sa meni healthbar, hungerbar, infectionbar
* `health` - pociatocny zivot hraca
* `maxHealth` - maximalny zivot ktory hrac vie dosiahnut
* `hunger` - maximalny level jedla
* `foodConsumptionRate` - ako casto sa ma updatovat (znizovat) hungerbar
* `infection` - hodnota infectionbaru na zaciatku, ked sa dostane na 0 si infikovany
* `infectionDropRate` - ako casto sa ma updatovat infectionbar
* `infectionHealthDropAmount` - kolko zivota nam bude kazdy update infectionbaru ubudat ak sme infikovany
* `infectionRegenerationAmount` - ako rychlo sa nam regeneruje infectionbar ak nie sme v kontaminovanej oblasti
### Environment
nejake veci suvisiace s hernym prostredim (zatial tu toho vela nie je, casom to tu bude pribudat)
## ContaminedLocation
Tymto vies zadefinovat kontaminovanu lokaciu (kde bude klesat infectionbar). Lokaciu zadefinujes tak, ze pridas hocijaky 2d collider a zakliknes `isTrigger`
* `infectionStrength` - sila infikovania (o kolko klesne infectionbar ak je hrac vo vnutri)
