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
