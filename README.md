# FarmacySystem – Dokumentacja Techniczna

## Spis treści

1. [Opis projektu](#1-opis-projektu)
2. [Wymagania systemowe](#2-wymagania-systemowe)
3. [Architektura aplikacji](#3-architektura-aplikacji)
4. [Struktura projektu](#4-struktura-projektu)
5. [Baza danych](#5-baza-danych)
6. [Modele danych](#6-modele-danych)
7. [Warstwa dostępu do danych (Repositories)](#7-warstwa-dostępu-do-danych-repositories)
8. [Formularze (UI)](#8-formularze-ui)
9. [Klasy pomocnicze (Helpers)](#9-klasy-pomocnicze-helpers)
10. [Konfiguracja](#10-konfiguracja)
11. [Przepływ danych – proces sprzedaży](#11-przepływ-danych--proces-sprzedaży)
12. [Walidacja danych](#12-walidacja-danych)
13. [Uruchomienie projektu](#13-uruchomienie-projektu)

---

## 1. Opis projektu

**FarmacySystem** to desktopowa aplikacja do zarządzania apteką, napisana w języku C# z użyciem technologii Windows Forms (.NET Framework 4.7.2). System umożliwia kompleksową obsługę apteki, w tym zarządzanie:

- lokalizacjami aptek,
- pracownikami,
- klientami,
- lekami,
- receptami,
- stanami magazynowymi,
- procesem sprzedaży.

Aplikacja korzysta z relacyjnej bazy danych **Microsoft SQL Server** (SQL Express) i realizuje architekturę trójwarstwową: warstwa prezentacji (formularze), warstwa logiki/dostępu do danych (adaptery) i warstwa modeli.

---

## 2. Wymagania systemowe

| Komponent | Wymaganie |
|---|---|
| System operacyjny | Windows 7/8/10/11 (x86/x64) |
| .NET Framework | 4.7.2 lub nowszy |
| Baza danych | Microsoft SQL Server Express (lokalny) |
| IDE (opcjonalnie) | Visual Studio 2017 lub nowszy |

---

## 3. Architektura aplikacji

Projekt stosuje trójwarstwową architekturę:

```
┌────────────────────────────────────────┐
│           Warstwa prezentacji          │
│  (Windows Forms – *Form.cs)            │
├────────────────────────────────────────┤
│     Warstwa dostępu do danych          │
│  (Repositories – *DataAdapter.cs)      │
├────────────────────────────────────────┤
│         Warstwa modeli                 │
│  (Models – *.cs)                       │
├────────────────────────────────────────┤
│       Microsoft SQL Server             │
│  (baza danych: Apteka)                 │
└────────────────────────────────────────┘
```

### Wzorce projektowe

- **DataSet / DataAdapter** – odczyt i zapis danych przez `SqlDataAdapter` i `SqlCommandBuilder`; dane przechowywane są lokalnie w `DataSet` przed wysłaniem do bazy.
- **Repository Pattern** – każda encja biznesowa posiada dedykowany adapter odpowiedzialny za CRUD.
- **Transaction** – operacja sprzedaży wykonywana jest w ramach transakcji SQL, aby zapewnić spójność danych.

---

## 4. Struktura projektu

```
FarmacySystem/
│
├── App.config                        # Konfiguracja połączenia z bazą danych
├── FarmacySystem.csproj              # Plik projektu MSBuild
├── Program.cs                        # Punkt wejścia aplikacji
│
├── Models/                           # Modele encji domenowych
│   ├── Klient.cs
│   ├── Lek.cs
│   ├── Lokalizacja.cs
│   ├── Pracownik.cs
│   ├── Recepta.cs
│   └── StanMagazynowy.cs
│
├── Repositories/                     # Warstwa dostępu do danych
│   ├── KlientDataAdapter.cs
│   ├── LekDataAdapter.cs
│   ├── LokalizacjaDataAdapter.cs
│   ├── PracownikDataAdapter.cs
│   ├── ReceptaDataAdapter.cs
│   ├── SprzedazDataAdapter.cs
│   └── StanMagazynowyDataAdapter.cs
│
├── Helpers/                          # Klasy pomocnicze
│   └── ComboBoxHelper.cs
│
├── MainMenuForm.cs / .Designer.cs    # Główne menu
├── KlientDataSetForm.cs              # Formularz klientów
├── LekDataSetForm.cs                 # Formularz leków
├── LokalizacjaDataSetForm.cs / .Designer.cs  # Formularz lokalizacji
├── PracownikDataSetForm.cs           # Formularz pracowników
├── ReceptaDataSetForm.cs             # Formularz recept
├── SprzedazForm.cs                   # Formularz sprzedaży
├── StanMagazynowyDataSetForm.cs      # Formularz stanów magazynowych
│
└── Properties/                       # Zasoby i metadane projektu
    ├── AssemblyInfo.cs
    ├── Resources.resx / .Designer.cs
    └── Settings.settings / .Designer.cs
```

---

## 5. Baza danych

### Nazwa bazy danych

`Apteka`

### Połączenie

```xml
Data Source=localhost\SQLEXPRESS;
Initial Catalog=Apteka;
Integrated Security=True;
```

Uwierzytelnianie realizowane jest przez **Windows Authentication** (Integrated Security).

### Schemat tabel

#### Tabela `klienci`

| Kolumna | Typ | Opis |
|---|---|---|
| `id` | INT (PK, IDENTITY) | Unikalny identyfikator klienta |
| `imie` | NVARCHAR | Imię klienta |
| `nazwisko` | NVARCHAR | Nazwisko klienta |
| `pesel` | CHAR(11) | Numer PESEL (11 cyfr) |
| `telefon` | NVARCHAR | Numer telefonu (opcjonalny) |
| `czy_ubezpieczony` | BIT | Flaga ubezpieczenia zdrowotnego |

#### Tabela `leki`

| Kolumna | Typ | Opis |
|---|---|---|
| `id` | INT (PK, IDENTITY) | Unikalny identyfikator leku |
| `nazwa_handlowa` | NVARCHAR | Nazwa handlowa leku |
| `substancja_czynna` | NVARCHAR | Substancja czynna |
| `producent` | NVARCHAR | Producent (opcjonalny) |
| `cena_brutto` | DECIMAL | Cena brutto (> 0) |
| `czy_na_recepte` | BIT | Flaga: czy lek jest na receptę |

#### Tabela `lokalizacje`

| Kolumna | Typ | Opis |
|---|---|---|
| `id` | INT (PK, IDENTITY) | Unikalny identyfikator lokalizacji |
| `nazwa` | NVARCHAR | Nazwa apteki/oddziału |
| `miasto` | NVARCHAR | Miasto |
| `adres` | NVARCHAR | Adres ulicy |
| `telefon` | NVARCHAR | Numer telefonu (opcjonalny) |

#### Tabela `pracownicy`

| Kolumna | Typ | Opis |
|---|---|---|
| `id` | INT (PK, IDENTITY) | Unikalny identyfikator pracownika |
| `imie` | NVARCHAR | Imię |
| `nazwisko` | NVARCHAR | Nazwisko |
| `pesel` | CHAR(11) | Numer PESEL |
| `email` | NVARCHAR | E-mail (opcjonalny) |
| `stanowisko` | NVARCHAR | Stanowisko (np. `lekarz`, `farmaceuta`) |
| `id_lokalizacji` | INT (FK → `lokalizacje.id`) | Przypisana lokalizacja |
| `data_zatrudnienia` | DATE | Data zatrudnienia (nie może być przyszłą) |

#### Tabela `recepty`

| Kolumna | Typ | Opis |
|---|---|---|
| `id` | INT (PK, IDENTITY) | Unikalny identyfikator recepty |
| `numer_recepty` | NVARCHAR | Numer recepty (unikalny) |
| `id_klienta` | INT (FK → `klienci.id`) | Klient |
| `id_leku` | INT (FK → `leki.id`) | Przepisany lek (musi mieć `czy_na_recepte = 1`) |
| `data_wystawienia` | DATE | Data wystawienia |
| `data_waznosci` | DATE | Data ważności (późniejsza niż data wystawienia) |
| `lekarz_imie_nazwisko` | NVARCHAR | Imię i nazwisko lekarza wystawiającego |
| `czy_zrealizowana` | BIT | Flaga realizacji (0 = otwarta, 1 = zrealizowana) |

#### Tabela `stany_magazynowe`

| Kolumna | Typ | Opis |
|---|---|---|
| `id` | INT (PK, IDENTITY) | Unikalny identyfikator wpisu |
| `id_lokalizacji` | INT (FK → `lokalizacje.id`) | Lokalizacja magazynu |
| `id_leku` | INT (FK → `leki.id`) | Lek |
| `ilosc` | INT | Ilość (≥ 0) |
| `data_waznosci` | DATE NULL | Data ważności partii (opcjonalna) |
| `status` | NVARCHAR | Status: `dostępny`, `zarezerwowany`, `niedostępny` |

### Diagram relacji (ERD)

```
lokalizacje ──< pracownicy
lokalizacje ──< stany_magazynowe
klienci     ──< recepty
leki        ──< recepty
leki        ──< stany_magazynowe
```

---

## 6. Modele danych

Modele w przestrzeni nazw `FarmacySystem.Models` to proste klasy POCO (Plain Old CLR Objects) służące jako kontenery danych.

### `Klient`

```csharp
public class Klient
{
    public int Id { get; set; }
    public string Imie { get; set; }
    public string Nazwisko { get; set; }
    public string Pesel { get; set; }
    public string Telefon { get; set; }
    public bool CzyUbezpieczony { get; set; }
}
```

### `Lek`

```csharp
public class Lek
{
    public int Id { get; set; }
    public string NazwaHandlowa { get; set; }
    public string SubstancjaCzynna { get; set; }
    public string Producent { get; set; }
    public decimal CenaBrutto { get; set; }
    public bool CzyNaRecepte { get; set; }
}
```

### `Lokalizacja`

```csharp
public class Lokalizacja
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public string Miasto { get; set; }
    public string Adres { get; set; }
    public string Telefon { get; set; }
}
```

### `Pracownik`

```csharp
public class Pracownik
{
    public int Id { get; set; }
    public string Imie { get; set; }
    public string Nazwisko { get; set; }
    public string Pesel { get; set; }
    public string Email { get; set; }
    public string Stanowisko { get; set; }
    public int IdLokalizacji { get; set; }
    public DateTime DataZatrudnienia { get; set; }

    // Pole obliczane (z JOIN)
    public string NazwaLokalizacji { get; set; }
}
```

### `Recepta`

```csharp
public class Recepta
{
    public int Id { get; set; }
    public string NumerRecepty { get; set; }
    public int IdKlienta { get; set; }
    public int IdLeku { get; set; }
    public DateTime DataWystawienia { get; set; }
    public DateTime DataWaznosci { get; set; }
    public string LekarzImieNazwisko { get; set; }
    public bool CzyZrealizowana { get; set; }

    // Pola obliczane (z JOIN)
    public string KlientImieNazwisko { get; set; }
    public string NazwaLeku { get; set; }
}
```

### `StanMagazynowy`

```csharp
public class StanMagazynowy
{
    public int Id { get; set; }
    public int IdLokalizacji { get; set; }
    public int IdLeku { get; set; }
    public int Ilosc { get; set; }
    public DateTime? DataWaznosci { get; set; }
    public string Status { get; set; }

    // Pola obliczane (z JOIN)
    public string NazwaLokalizacji { get; set; }
    public string NazwaLeku { get; set; }
}
```

---

## 7. Warstwa dostępu do danych (Repositories)

Wszystkie adaptery znajdują się w przestrzeni nazw `FarmacySystem.Repositories`. Każdy adapter odczytuje connection string `AptekaConnectionString` z `App.config`.

### Wspólny wzorzec adapterów CRUD

Każdy adapter CRUD (`KlientDataAdapter`, `LekDataAdapter`, `LokalizacjaDataAdapter`, `PracownikDataAdapter`, `ReceptaDataAdapter`, `StanMagazynowyDataAdapter`) implementuje trzy metody:

| Metoda | Zwraca | Opis |
|---|---|---|
| `PobierzDataSet()` | `DataSet` | Pobiera dane z bazy do `DataSet`; konfiguruje `AutoIncrement` i klucz główny |
| `ZapiszZmiany(DataSet)` | `int` | Wysyła zmiany (`Added`, `Modified`, `Deleted`) z `DataSet` do bazy; zwraca liczbę zmienionych wierszy |
| `Waliduj(DataRow, out string)` | `bool` | Waliduje pojedynczy wiersz przed zapisem; w przypadku błędu ustawia komunikat w parametrze `out` |

#### Mechanizm AutoIncrement

Podczas ładowania danych adapter:
1. Włącza `AutoIncrement = true` na kolumnie `id`.
2. Oblicza `MAX(id)` w tabeli i ustawia `AutoIncrementSeed = maxId + 1`, aby unikać konfliktów z ID generowanymi przez bazę.
3. Konfiguruje `InsertCommand` tak, aby po wstawieniu rekordu baza zwracała nowe `id` (przez `SCOPE_IDENTITY()`), aktualizując tym samym wiersz w `DataSet`.

---

### `KlientDataAdapter`

**Plik:** `Repositories/KlientDataAdapter.cs`

**Zapytanie SELECT:**
```sql
SELECT id, imie, nazwisko, pesel, telefon, czy_ubezpieczony
FROM klienci
ORDER BY nazwisko, imie
```

**Walidacja:**
- `imie` – wymagane
- `nazwisko` – wymagane
- `pesel` – wymagane, dokładnie 11 cyfr, poprawna suma kontrolna
- `telefon` – opcjonalny; jeśli podany: tylko cyfry/spacje/myślniki/+, długość 9–15 cyfr

---

### `LekDataAdapter`

**Plik:** `Repositories/LekDataAdapter.cs`

**Zapytanie SELECT:**
```sql
SELECT id, nazwa_handlowa, substancja_czynna, producent, cena_brutto, czy_na_recepte
FROM leki
ORDER BY nazwa_handlowa
```

**Walidacja:**
- `nazwa_handlowa` – wymagana
- `substancja_czynna` – wymagana
- `cena_brutto` – wymagana, wartość > 0

---

### `LokalizacjaDataAdapter`

**Plik:** `Repositories/LokalizacjaDataAdapter.cs`

**Zapytanie SELECT:**
```sql
SELECT id, nazwa, miasto, adres, telefon
FROM lokalizacje
ORDER BY nazwa
```

**Walidacja:**
- `nazwa` – wymagana
- `miasto` – wymagane
- `adres` – wymagany
- `telefon` – opcjonalny; jeśli podany: tylko cyfry/spacje/myślniki/+, długość 9–15 cyfr

---

### `PracownikDataAdapter`

**Plik:** `Repositories/PracownikDataAdapter.cs`

**Zapytanie SELECT (z JOIN):**
```sql
SELECT p.id, p.imie, p.nazwisko, p.pesel, p.email, p.stanowisko,
       p.id_lokalizacji, p.data_zatrudnienia,
       l.nazwa AS nazwa_lokalizacji
FROM pracownicy p
INNER JOIN lokalizacje l ON p.id_lokalizacji = l.id
ORDER BY p.nazwisko, p.imie
```

Ładuje również tabelę pomocniczą `Lokalizacje` (dla ComboBox w formularzu).

**Zapis:** Przed wywołaniem `adapter.Update()` kolumna `nazwa_lokalizacji` jest usuwana z kopii tabeli (pochodzi z JOIN, nie istnieje w tabeli `pracownicy`).

**Walidacja:**
- `imie`, `nazwisko` – wymagane
- `pesel` – wymagany, 11 cyfr, poprawna suma kontrolna
- `email` – opcjonalny; jeśli podany: musi zawierać `@` i `.`
- `stanowisko` – wymagane
- `id_lokalizacji` – wymagane
- `data_zatrudnienia` – wymagana, nie może być w przyszłości

---

### `ReceptaDataAdapter`

**Plik:** `Repositories/ReceptaDataAdapter.cs`

**Zapytanie SELECT (z JOIN):**
```sql
SELECT r.id, r.numer_recepty, r.id_klienta, r.id_leku,
       r.data_wystawienia, r.data_waznosci, r.lekarz_imie_nazwisko, r.czy_zrealizowana,
       k.imie + ' ' + k.nazwisko AS klient_imie_nazwisko,
       l.nazwa_handlowa AS nazwa_leku
FROM recepty r
INNER JOIN klienci k ON r.id_klienta = k.id
INNER JOIN leki l ON r.id_leku = l.id
ORDER BY r.data_wystawienia DESC
```

Ładuje tabele pomocnicze: `Klienci`, `Leki` (tylko leki na receptę: `czy_na_recepte = 1`), `Lekarze` (pracownicy ze stanowiskiem `lekarz`).

**Zapis:** Usuwa kolumny `klient_imie_nazwisko` i `nazwa_leku` z kopii tabeli przed zapisem.

**Walidacja:**
- `numer_recepty` – wymagany
- `id_klienta`, `id_leku` – wymagane
- `data_wystawienia`, `data_waznosci` – wymagane; data ważności > data wystawienia
- `lekarz_imie_nazwisko` – wymagane

---

### `StanMagazynowyDataAdapter`

**Plik:** `Repositories/StanMagazynowyDataAdapter.cs`

**Zapytanie SELECT (z JOIN):**
```sql
SELECT s.id, s.id_lokalizacji, s.id_leku, s.ilosc, s.data_waznosci, s.status,
       lok.nazwa AS nazwa_lokalizacji,
       l.nazwa_handlowa AS nazwa_leku
FROM stany_magazynowe s
INNER JOIN lokalizacje lok ON s.id_lokalizacji = lok.id
INNER JOIN leki l ON s.id_leku = l.id
ORDER BY lok.nazwa, l.nazwa_handlowa
```

Ładuje tabele pomocnicze: `Lokalizacje`, `Leki`.

**Walidacja:**
- `id_lokalizacji`, `id_leku` – wymagane
- `ilosc` – wymagana, wartość ≥ 0
- `status` – jeśli podany: musi być jednym z `dostępny`, `zarezerwowany`, `niedostępny`

---

### `SprzedazDataAdapter`

**Plik:** `Repositories/SprzedazDataAdapter.cs`

Adapter dedykowany procesowi sprzedaży. Nie stosuje wzorca CRUD – zamiast tego udostępnia wyspecjalizowane metody:

#### `PobierzDaneDoSprzedazy() : DataSet`

Zwraca `DataSet` z tabelami: `Lokalizacje`, `Klienci`, `Leki` – służą do wypełnienia list wyboru w formularzu sprzedaży.

#### `PobierzStanMagazynowy(int idLokalizacji, int idLeku) : int`

Zwraca sumę ilości dostępnych (`status = 'dostępny'`) sztuk danego leku w danej lokalizacji:

```sql
SELECT ISNULL(SUM(ilosc), 0)
FROM stany_magazynowe
WHERE id_lokalizacji = @idLokalizacji
  AND id_leku = @idLeku
  AND status = 'dostępny'
```

#### `KlientPosiadaRecepteNaLek(int idKlienta, int idLeku) : bool`

Sprawdza, czy klient posiada niezrealizowaną, ważną receptę na dany lek:

```sql
SELECT COUNT(*)
FROM recepty
WHERE id_klienta = @idKlienta
  AND id_leku = @idLeku
  AND czy_zrealizowana = 0
  AND data_waznosci >= GETDATE()
```

#### `ZrealizujSprzedaz(int idKlienta, int idLeku, int idLokalizacji, int ilosc, decimal kwota, out int idReceptyDoZrealizowania) : bool`

Realizuje sprzedaż w ramach jednej transakcji SQL:

1. Wyszukuje najstarszą ważną, niezrealizowaną receptę klienta na dany lek (`TOP 1 ... ORDER BY data_waznosci ASC`).
2. Oznacza receptę jako zrealizowaną (`czy_zrealizowana = 1`), jeśli taka istnieje.
3. Zmniejsza ilość w `stany_magazynowe` o zamówioną `ilosc` (tylko rekordy `status = 'dostępny'`).
4. Jeśli stan magazynowy nie może być zmniejszony – wykonuje `ROLLBACK` i zgłasza wyjątek.
5. W przypadku powodzenia – `COMMIT`.

---

## 8. Formularze (UI)

### `MainMenuForm`

**Plik:** `MainMenuForm.cs`

Główne okno aplikacji. Zawiera przyciski nawigacyjne otwierające kolejne formularze jako okna modalne (`ShowDialog()`):

| Przycisk | Otwierany formularz |
|---|---|
| Lokalizacje | `LokalizacjaDataSetForm` |
| Pracownicy | `PracownikDataSetForm` |
| Klienci | `KlientDataSetForm` |
| Leki | `LekDataSetForm` |
| Recepty | `ReceptaDataSetForm` |
| Magazyn | `StanMagazynowyDataSetForm` |
| Sprzedaż | `SprzedazForm` |
| Wyjście | `Application.Exit()` |

---

### Formularze CRUD (`*DataSetForm`)

Formularze: `KlientDataSetForm`, `LekDataSetForm`, `LokalizacjaDataSetForm`, `PracownikDataSetForm`, `ReceptaDataSetForm`, `StanMagazynowyDataSetForm` stosują wspólny wzorzec oparty na `DataGridView` powiązanym z `DataSet`:

- **Załadowanie danych** – wywołanie `PobierzDataSet()` odpowiedniego adaptera.
- **Dodawanie/edycja** – bezpośrednio w `DataGridView`; zmienione wiersze są oznaczane przez `DataSet` jako `Added`/`Modified`.
- **Usuwanie** – zaznaczony wiersz usuwany z `DataTable` (stan `Deleted`).
- **Zapis** – wywołanie `Waliduj()` dla każdego zmienionego wiersza, a następnie `ZapiszZmiany(dataSet)`.
- **Odświeżenie** – ponowne wywołanie `PobierzDataSet()`.

Formularze zawierające pola relacyjne (`Pracownicy`, `Recepty`, `StanMagazynowy`) ładują dodatkowe tabele pomocnicze do `DataSet` i używają `ComboBox` powiązanego z tymi tabelami.

---

### `SprzedazForm`

**Plik:** `SprzedazForm.cs`

Formularz realizacji sprzedaży. Rozmiar: 1000×700 px.

**Elementy interfejsu:**

| Element | Typ | Opis |
|---|---|---|
| `cmbLokalizacja` | ComboBox | Wybór lokalizacji apteki |
| `cmbKlient` | ComboBox | Wybór klienta |
| `cmbLek` | ComboBox | Wybór leku |
| `chkCzyNaRecepte` | CheckBox (tylko do odczytu) | Informacja: czy lek wymaga recepty |
| `txtStanMagazynowy` | TextBox (tylko do odczytu) | Aktualny stan magazynowy |
| `txtIlosc` | TextBox | Żądana ilość sztuk |
| `txtCenaJednostkowa` | TextBox (tylko do odczytu) | Cena jednostkowa leku |
| `txtRazem` | TextBox (tylko do odczytu) | Kwota do zapłaty |
| `lblReceptaStatus` | Label | Status recepty klienta |
| `btnDodaj` | Button | Dodanie pozycji do koszyka |
| `btnZrealizuj` | Button | Finalizacja zakupu |
| `btnAnuluj` | Button | Anulowanie koszyka |
| `dgvKoszyk` | DataGridView | Zawartość koszyka zakupów |
| `lblSumaCalkowita` | Label | Suma całkowita koszyka |

**Logika działania:**

1. Przy zmianie lokalizacji i leku – automatyczne pobranie stanu magazynowego.
2. Przy zmianie klienta i leku – sprawdzenie recepty (`KlientPosiadaRecepteNaLek`).
3. Przy zmianie ilości – przeliczenie kwoty `Razem = Ilość × CenaJednostkowa`.
4. `Dodaj do koszyka` – walidacja (dostępność, recepta) i dodanie pozycji do `dtKoszyk`.
5. `Zrealizuj` – iteracja po koszyku i wywołanie `ZrealizujSprzedaz()` dla każdej pozycji.

---

## 9. Klasy pomocnicze (Helpers)

### `ComboBoxItem`

**Plik:** `Helpers/ComboBoxHelper.cs`

Prosta klasa do przechowywania par klucz-wartość dla kontrolek `ComboBox`:

```csharp
public class ComboBoxItem
{
    public int Value { get; set; }      // ID rekordu w bazie
    public string Display { get; set; } // Tekst wyświetlany w ComboBox

    public override string ToString() => Display;
}
```

### `ComboBoxHelper` (statyczna)

Statyczna klasa oferująca metody pobierające listy do wypełnienia `ComboBox`ów w formularzach.

| Metoda | Zapytanie SQL | Zwraca |
|---|---|---|
| `PobierzLokalizacje()` | `SELECT id, nazwa FROM lokalizacje ORDER BY nazwa` | `List<ComboBoxItem>` |
| `PobierzKlientow()` | `SELECT id, imie + ' ' + nazwisko FROM klienci ORDER BY nazwisko, imie` | `List<ComboBoxItem>` |
| `PobierzLeki()` | `SELECT id, nazwa_handlowa FROM leki ORDER BY nazwa_handlowa` | `List<ComboBoxItem>` |

Każda metoda otwiera własne połączenie z bazą i zwraca listę obiektów `ComboBoxItem`.

---

## 10. Konfiguracja

### `App.config`

```xml
<configuration>
  <connectionStrings>
    <add name="AptekaConnectionString"
         connectionString="Data Source=localhost\SQLEXPRESS;Initial Catalog=Apteka;Integrated Security=True;"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

**Parametry połączenia:**

| Parametr | Wartość | Opis |
|---|---|---|
| `Data Source` | `localhost\SQLEXPRESS` | Instancja serwera SQL (lokalny SQL Express) |
| `Initial Catalog` | `Apteka` | Nazwa bazy danych |
| `Integrated Security` | `True` | Uwierzytelnianie Windows (brak hasła) |

> **Uwaga:** Aby uruchomić aplikację na innym serwerze, należy zmienić `Data Source` w `App.config`. W przypadku uwierzytelniania SQL Server należy zastąpić `Integrated Security=True` parametrami `User ID=...;******

---

## 11. Przepływ danych – proces sprzedaży

Poniżej przedstawiony jest szczegółowy przepływ realizacji sprzedaży:

```
[SprzedazForm]
      │
      ├─ Wybór lokalizacji, klienta, leku, ilości
      │
      ├─ SprzedazDataAdapter.PobierzStanMagazynowy()
      │       └─ Sprawdza dostępność w stany_magazynowe
      │
      ├─ SprzedazDataAdapter.KlientPosiadaRecepteNaLek()
      │       └─ Sprawdza ważne, niezrealizowane recepty
      │
      ├─ [Dodaj do koszyka]
      │       └─ Dodanie wiersza do dtKoszyk (lokalny DataTable)
      │
      └─ [Zrealizuj]
              └─ SprzedazDataAdapter.ZrealizujSprzedaz()
                      ├─ BEGIN TRANSACTION
                      ├─ SELECT TOP 1 recepta (jeśli wymagana)
                      ├─ UPDATE recepty SET czy_zrealizowana = 1
                      ├─ UPDATE stany_magazynowe SET ilosc = ilosc - @ilosc
                      │       └─ Jeśli rowsAffected = 0 → ROLLBACK + Exception
                      └─ COMMIT
```

---

## 12. Walidacja danych

### Algorytm weryfikacji sumy kontrolnej PESEL

Używany w `KlientDataAdapter` i `PracownikDataAdapter`:

```
Wagi: [1, 3, 7, 9, 1, 3, 7, 9, 1, 3]
Suma = Σ (cyfra[i] × waga[i]) dla i = 0..9
Suma kontrolna = (10 − (Suma mod 10)) mod 10
Wynik: Suma kontrolna == cyfra[10] (ostatnia cyfra PESEL)
```

### Walidacja telefonu

Stosowana w `KlientDataAdapter` i `LokalizacjaDataAdapter`:
- Usuwane są spacje, myślniki i znak `+`.
- Pozostałe znaki muszą być cyframi.
- Długość po oczyszczeniu: 9–15 cyfr.

### Walidacja email (Pracownik)

Uproszczona: adres musi zawierać znaki `@` oraz `.`.

### Statusy stanu magazynowego

Dopuszczalne wartości: `dostępny`, `zarezerwowany`, `niedostępny` (porównanie case-insensitive).

---

## 13. Uruchomienie projektu

### Krok 1: Przygotowanie bazy danych

1. Zainstaluj **Microsoft SQL Server Express** (lub wyższy).
2. Utwórz bazę danych `Apteka`:
   ```sql
   CREATE DATABASE Apteka;
   ```
3. Utwórz tabele zgodnie ze schematem opisanym w [rozdziale 5](#5-baza-danych).

### Krok 2: Konfiguracja połączenia

Otwórz plik `App.config` i dostosuj parametr `Data Source` do nazwy swojej instancji SQL Server, np.:
```xml
Data Source=NAZWA_KOMPUTERA\SQLEXPRESS;
```

### Krok 3: Kompilacja i uruchomienie

1. Otwórz `FarmacySystem.csproj` w **Visual Studio 2017+**.
2. Zbuduj projekt: **Build → Build Solution** (`Ctrl+Shift+B`).
3. Uruchom: **Debug → Start Debugging** (`F5`) lub **Start Without Debugging** (`Ctrl+F5`).

Aplikacja uruchomi się od `MainMenuForm` – głównego menu nawigacyjnego.

---

*Dokumentacja wygenerowana na podstawie analizy kodu źródłowego projektu FarmacySystem.*
