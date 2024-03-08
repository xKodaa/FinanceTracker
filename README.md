# FinanceTracker

## TODO
- [x] Implementovat register
- [x] Implementovat login
- [x] Rozvrhnout layout aplikce
- [x] Přidat možnost vytvoření struktury databáze při spuštění aplikace
- [x] Přidat rychlost vyčítání dat z API do konfiguračního souboru
- [ ] Vytvořit layout Profile page
- [ ] Vytvořit layout Finance page
- [ ] Vytvořit layout Akcie page
- [ ] Vytvořit layout Kryptoměny page
- [ ] Vytvořit layout Dashboard page
- [ ] Implementovat logiku na pozadí jednotlivých pages

## NOTES

### POKUD BUDU NAKONEC ŘEŠIT AKCIE
- Nasadit vyhledávání ticker symbolu do Akcie Page, podle toho se budou provádět dotazy na Stocks API
- Nějakým způsobem profiltrovat Tickers...
  
### OBECNÉ
- Možnost konverze mezi USDT = CZK
- Při registraci požádat o vyplnění uživatele o jeho preferované měně, pár oblíbených kryptoměn/akcií
- Ke kryptu dát možnost konvertovat měny (aby se hodnoty kryptoměn zobrazovaly v jinych měnach) 
- Možná přidat sekci konvertor? Nacpat zde všechny dostupné měny z API a možnost mezi nimi libovolně konvertovat.

## LINKS
- Tickers: https://www.nasdaq.com/market-activity/stocks/screener
- Crypto API: https://docs.coincap.io/#89deffa0-ab03-4e0a-8d92-637a857d2c91
- Stocks API: https://polygon.io/docs/stocks/get_v2_aggs_ticker__stocksticker__range__multiplier___timespan___from___to, https://www.alphavantage.co/documentation/
- Currency convertor API: https://www.frankfurter.app/docs/
- Free APIs (useful): https://rapidapi.com/hub
