# RESTful services
## Rekomendacje przepisów

RecipeAPI to prosty serwer API oparty na ASP.NET Core, który pozwala użytkownikom wyszukiwać przepisy na podstawie dostępnych składników oraz uzyskiwać informacje o wartości odżywczych dla tych przepisów.

## Wymagania
- .NET 5.0 lub nowszy
- Klucze API dla Spoonacular i Nutritionix (do umieszczenia w pliku .env)

## Instalacja

1. Sklonuj to repozytorium:
```git clone https://github.com/Seszele/Systemy-Rozproszone.git```
2. Przejdź do folderu projektu:
```cd .\Systemy-Rozproszone\lab2\RecipeAPI```

3. Utwórz plik `.env` w głównym folderze projektu z następującą zawartością, zamieniając wartości w nawiasach klamrowych na rzeczywiste klucze API:
```
SPOONACULAR_API_KEY={Twój_klucz_API_Spoonacular}
SPOONACULAR_API_BASE=https://api.spoonacular.com/
NUTRITIONIX_APP_ID={Twój_app_ID_Nutritionix}
NUTRITIONIX_APP_KEY={Twój_klucz_API_Nutritionix}
NUTRITIONIX_API_BASE=https://trackapi.nutritionix.com/
```

4. Zbuduj i uruchom projekt:
```
dotnet build
dotnet run
```


## Użycie

Aplikacja wystawia jeden punkt końcowy API:

1. `GET /api/Recipe?ingredients={lista_składników}` - wyszukuje przepis na podstawie listy składników (oddzielonych przecinkami) i zwraca prostego htmla z przepisem i wartościami odżywczymi

Aplikacja korzysta z następujących zewnętrznych usług API:

1. [Spoonacular API](https://spoonacular.com/food-api) - do wyszukiwania przepisów
2. [Nutritionix API](https://www.nutritionix.com/business/api) - do pobierania informacji o wartościach odżywczych
