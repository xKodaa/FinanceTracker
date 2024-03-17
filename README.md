# FinanceTracker

## TODO
- [x] Implementovat register
- [x] Implementovat login
- [x] Rozvrhnout layout aplikce
- [x] Přidat možnost vytvoření struktury databáze při spuštění aplikace
- [x] Přidat rychlost vyčítání dat z API do konfiguračního souboru
- [ ] Vytvořit layout Profile page
- [x] Vytvořit layout Převodník page
- [x] Vytvořit layout Finance page
- [ ] Vytvořit layout Kryptoměny page
- [x] Vytvořit layout Dashboard page
- [ ] Implementovat logiku na pozadí jednotlivých pages
- [ ] Implementovat tabulky v DB kam se budou vkládat jednotlivé výdaje
- [ ] Implementovat tabulky v DB kam se budou dávat jednotlivé kryptoměny 
- [ ] Implementovat zobrazování grafů
      
## NOTES
  
### OBECNÉ
- Vymyslet implementaci profilu, asi jim vypsat jejich preference (last login / info z db...)
- Při registraci požádat o vyplnění uživatele o jeho preferované měně, pár oblíbených kryptoměn/akcií - register window (MOŽNÁ)
- Ke kryptu dát možnost konvertovat měny (aby se hodnoty kryptoměn zobrazovaly v jinych měnach) - využít stejné metody jako konvertor (MOŽNÁ)
- Dashboards combobox na roky dát dynamicky dle nejstarších dat v DB
- Do Financí po odeslání financí z těch dat udělat objekt, který se následně dobrazuje do grafu

## LINKS
- Crypto API: https://docs.coincap.io/#89deffa0-ab03-4e0a-8d92-637a857d2c91
- Currency convertor API: https://www.frankfurter.app/docs/
