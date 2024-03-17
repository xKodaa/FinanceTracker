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
- [ ] Vytvořit layout Dashboard page
- [ ] Implementovat logiku na pozadí jednotlivých pages

## NOTES
  
### OBECNÉ
- Možnost konverze mezi USDT = CZK - převodník page
- Při registraci požádat o vyplnění uživatele o jeho preferované měně, pár oblíbených kryptoměn/akcií - register window
- Ke kryptu dát možnost konvertovat měny (aby se hodnoty kryptoměn zobrazovaly v jinych měnach) - využít stejné metody jako konvertor
- Do dashboards dát comboboxy na měsíce a roky a udělat dotazy do databáze na tyto parametry, dle toho vyplňovat grafy

## LINKS
- Tickers: https://www.nasdaq.com/market-activity/stocks/screener
- Crypto API: https://docs.coincap.io/#89deffa0-ab03-4e0a-8d92-637a857d2c91
- Stocks API: https://polygon.io/docs/stocks/get_v2_aggs_ticker__stocksticker__range__multiplier___timespan___from___to, https://www.alphavantage.co/documentation/
- Currency convertor API: https://www.frankfurter.app/docs/
- Free APIs (useful): https://rapidapi.com/hub
