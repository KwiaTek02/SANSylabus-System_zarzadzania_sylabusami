CREATE DATABASE SANSylabusDb;
GO


-- ==================================================================
-- 1. Tabela uzytkownicy
-- ==================================================================
CREATE TABLE dbo.uzytkownicy (
    id                INT IDENTITY PRIMARY KEY,
    imie_nazwisko     NVARCHAR(255) NOT NULL,
    tytul             NVARCHAR(50)  NULL,
    login             NVARCHAR(100) NOT NULL UNIQUE,
    haslo             NVARCHAR(255) NOT NULL,
    sol               VARCHAR(24) NOT NULL,
    email             NVARCHAR(150) NOT NULL UNIQUE,
    typ_konta         NVARCHAR(20)  NOT NULL
      CONSTRAINT chk_uzytkownicy_typ_konta
        CHECK(typ_konta IN ('gosc','student','wykladowca','admin'))
);
GO

-- ==================================================================
-- 2. Tabela przedmioty 
-- ==================================================================
CREATE TABLE dbo.przedmioty (
    id        INT IDENTITY PRIMARY KEY,
    nazwa     NVARCHAR(255) NOT NULL,
    osrodek   NVARCHAR(100) NULL,
    semestr   TINYINT       NULL,
    stopien   NVARCHAR(50)  NULL,
    kierunek  NVARCHAR(100) NULL,
    suma_godzin_calosciowe INT NULL
);
GO

-- ==================================================================
-- 3. Tabela sylabusy z JSON-ami zamiast kilku osobnych tabel
-- ==================================================================
CREATE TABLE dbo.sylabusy (
    id                             INT IDENTITY PRIMARY KEY,
    przedmiot_id                   INT NOT NULL,
    wersja                         NVARCHAR(20) NOT NULL,
    nazwa_jednostki_organizacyjnej NVARCHAR(255) NULL,
    profil_ksztalcenia             NVARCHAR(255) NULL,
    nazwa_specjalnosci             NVARCHAR(255) NULL,
    rodzaj_modulu_ksztalcenia      NVARCHAR(255) NULL,
    wymagania_wstepne              NVARCHAR(MAX)  NULL,
    rok_data                       NVARCHAR(20)   NULL,  -- np. '2024/2025'
    data_powstania                 DATETIME       NULL,
    kto_stworzyl                   INT            NOT NULL,
 
    tresci_ksztalcenia_json        NVARCHAR(MAX)  NULL,
    efekty_ksztalcenia_json        NVARCHAR(MAX)  NULL, 
    metody_weryfikacji_json        NVARCHAR(MAX)  NULL,
    kryteria_oceny_json            NVARCHAR(MAX)  NULL,
    naklad_pracy_json              NVARCHAR(MAX)  NULL,  
    literatura_json                NVARCHAR(MAX)  NULL,
    metody_realizacji_json         NVARCHAR(MAX)  NULL  
);
GO

-- ==================================================================
-- 4. Tabela koordynatorzy_sylabusu 
-- ==================================================================
CREATE TABLE dbo.koordynatorzy_sylabusu (
    id            INT IDENTITY PRIMARY KEY,
    sylabus_id    INT NOT NULL,
    uzytkownik_id INT NOT NULL
);
GO

-- ==================================================================
-- 5. Tabela sylabus_historia
-- ==================================================================
CREATE TABLE dbo.sylabus_historia (
    id            INT IDENTITY PRIMARY KEY,
    sylabus_id    INT NOT NULL,
    data_zmiany   DATE    NOT NULL,
    czas_zmiany   DATETIME NOT NULL,
    zmieniony_przez INT   NOT NULL,
    opis_zmiany   NVARCHAR(MAX) NULL,
    wersja_wtedy  NVARCHAR(20)  NULL
);
GO


-- ==================================================================
-- 6. Tabela siatka_przedmiotow
-- ==================================================================
CREATE TABLE dbo.siatka_przedmiotow (
    id           INT IDENTITY PRIMARY KEY,
    przedmiot_id INT NOT NULL,
    typ          NVARCHAR(20) NOT NULL
      CONSTRAINT chk_siatka_typ
        CHECK(typ IN ('stacjonarne','niestacjonarne')),
    wyklad        INT NULL,
    cwiczenia     INT NULL,
    konwersatorium INT NULL,
    laboratorium  INT NULL,
    warsztaty     INT NULL,
    projekt       INT NULL,
    seminarium    INT NULL,
    konsultacje   INT NULL,
    egzaminy      INT NULL,
    sumagodzin    INT NULL
);
GO

-- ==================================================================
-- 8. Dodajemy klucze obce
-- ==================================================================
ALTER TABLE dbo.sylabusy
  ADD CONSTRAINT FK_sylabusy_kto_stworzyl
      FOREIGN KEY(kto_stworzyl) REFERENCES dbo.uzytkownicy(id);
GO
ALTER TABLE dbo.sylabusy
  ADD CONSTRAINT FK_sylabusy_przedmioty
      FOREIGN KEY(przedmiot_id) REFERENCES dbo.przedmioty(id);
GO

ALTER TABLE dbo.koordynatorzy_sylabusu
  ADD CONSTRAINT FK_koord_syl_sylabus
      FOREIGN KEY(sylabus_id) REFERENCES dbo.sylabusy(id);
ALTER TABLE dbo.koordynatorzy_sylabusu
  ADD CONSTRAINT FK_koord_syl_uzytkownicy
      FOREIGN KEY(uzytkownik_id) REFERENCES dbo.uzytkownicy(id);
GO

ALTER TABLE dbo.sylabus_historia
  ADD CONSTRAINT FK_historia_sylabus
      FOREIGN KEY(sylabus_id) REFERENCES dbo.sylabusy(id);
ALTER TABLE dbo.sylabus_historia
  ADD CONSTRAINT FK_historia_uzytkownicy
      FOREIGN KEY(zmieniony_przez) REFERENCES dbo.uzytkownicy(id);
GO

ALTER TABLE dbo.siatka_przedmiotow
  ADD CONSTRAINT FK_siatka_przedmiotow_przedmioty
      FOREIGN KEY(przedmiot_id) REFERENCES dbo.przedmioty(id);
GO

-- ==================================================================
-- Przykładowe wstawienie danych z PDF (tylko fragment dla id=1)
-- ==================================================================
INSERT INTO dbo.uzytkownicy (imie_nazwisko,tytul,login,haslo,sol,email,typ_konta) VALUES 
('Alina Marchlewska','dr','wykladowca1','lh/2YQpYKHaDAKs04RD82W/c/X+0oYcuDZVCoQqohFA=','y+jOPHX83bbbZqaXN8gD3g==','amarchlewska@san.pl','wykladowca'),
('Krystian Gumiński', 'imperator','wykladowca2','7vVN8gAsKogzdm/mPRdM/bdX9OobbOLmNQuoy4+FOH8=','2JuvDOnTglcPfTBsQErU2Q==','kguminski@san.edu.pl','wykladowca');
GO

INSERT INTO dbo.przedmioty (nazwa,osrodek,semestr,stopien,kierunek,suma_godzin_calosciowe)
VALUES ('Analiza matematyczna i algebra liniowa','Łódź',1,'1','Informatyka',125);
GO

INSERT INTO dbo.siatka_przedmiotow (przedmiot_id, typ, wyklad, cwiczenia, konwersatorium, laboratorium, warsztaty, projekt, seminarium, konsultacje, egzaminy, sumagodzin) VALUES 
(1, 'stacjonarne', 30, 30, 0, 0, 0, 0, 0, 0, 0, 60),
(1, 'niestacjonarne', 20, 20, 0, 0, 0, 0, 0, 0, 0, 40);
GO

INSERT INTO dbo.sylabusy
  (przedmiot_id,wersja,nazwa_jednostki_organizacyjnej,profil_ksztalcenia,nazwa_specjalnosci,rodzaj_modulu_ksztalcenia,wymagania_wstepne,rok_data,data_powstania,kto_stworzyl,tresci_ksztalcenia_json,efekty_ksztalcenia_json,metody_weryfikacji_json,kryteria_oceny_json,naklad_pracy_json,literatura_json,metody_realizacji_json)
VALUES
( 1,'v1','Wydział Studiów Międzynarodowych i Informatyki Społecznej Akademii Nauk w Łodzi','OGÓLNOAKADEMICKI','nie dotyczy','podstawowy',
  'Wiedza matematyczna z zakresu szkoły średniej','2024/2025','2025-06-08 21:37:00',1,

  -- tresci_ksztalcenia_json

  N'{"wyklady":[{"lp":1,"opis":"Macierze – działania na macierzach, wyznaczniki, rząd macierzy, diagonalizacja macierzy","odniesienie":["P_W01"]},{"lp":2,"opis":"Układy równań liniowych – zapis macierzowy, wzory Cramera, eliminacja Gaussa.","odniesienie":["P_W01","P_U01"]},{"lp":3,"opis":"Funkcje rzeczywiste jednej zmiennej (pojęcie funkcji, rodzaje i własności funkcji, ciągi liczbowe).","odniesienie":["P_U01","P_K01"]},{"lp":4,"opis":"Ciągi liczbowe – pojęcie ciągu liczbowego i podciągów, ich rodzaje, granica ciągu, własności ciągów zbieżnych.","odniesienie":["P_W01","P_U01"]},{"lp":5,"opis":"Granica funkcji, interpretacja geometryczna.","odniesienie":["P_W01"]},{"lp":6,"opis":"Funkcje wielu zmiennych.","odniesienie":["P_W01"]},{"lp":7,"opis":"Funkcje ciągłe i jednostajnie ciągłe, własności funkcji ciągłych.","odniesienie":["P_W01","P_U01"]},{"lp":8,"opis":"Pochodna funkcji, ekstrema (warunek konieczny i warunki wystarczające istnienia ekstremum), wypukłość i wklęsłość krzywej.","odniesienie":["P_W01","P_U01"]},{"lp":9,"opis":"Szeregi rzeczywiste (szeregi o wyrazach dodatnich, o wyrazach dowolnych i naprzemienne, kryteria zbieżności, bezwzględna zbieżność szeregu).","odniesienie":["P_W01","P_U01"]},{"lp":10,"opis":"Ciągi i szeregi funkcyjne, definicja zbieżności punktowej oraz zbieżności jednostajnej, kryteria zbieżności szeregów funkcyjnych.","odniesienie":["P_W01","P_U01"]},{"lp":11,"opis":"Rachunek całkowy – całka oznaczona i nieoznaczona, zastosowanie całek oznaczonych.","odniesienie":["P_W01","P_U01"]},{"lp":12,"opis":"Elementy geometrii analitycznej.","odniesienie":["P_W01","P_U01"]}],"cwiczenia":[{"lp":1,"opis":"Macierze – działania na macierzach, obliczanie wyznacznika, rzędu i śladu. Wyznaczanie macierzy odwrotnej.","odniesienie":["P_W01","P_U01"]},{"lp":2,"opis":"Układy równań liniowych – Twierdzenia Kroneckera-Capellego, układy Cramera, wyznaczanie rozwiązań ogólnych i przykładowych szczególnych, metoda eliminacji Gaussa.","odniesienie":["P_W01","P_U01","P_K01"]},{"lp":3,"opis":"Granica i ciągłość funkcji – obliczanie granic i badanie ciągłości funkcji. Zastosowanie granic do wyznaczania asymptot wykresu.","odniesienie":["P_W01","P_U01"]},{"lp":4,"opis":"Rachunek różniczkowy funkcji jednej zmiennej – wyznaczanie ekstremum lokalnego i badanie monotoniczności funkcji. Druga pochodna i jej zastosowanie do badania wklęsłości i wypukłości krzywej.","odniesienie":["P_W01","P_U01"]},{"lp":5,"opis":"Badanie przebiegu zmienności funkcji i rysowanie jej wykresu.","odniesienie":["P_U01"]},{"lp":6,"opis":"Szeregi liczbowe – obliczanie sum pewnych szeregów. Szeregi geometryczne – ocena zbieżności i obliczanie sumy. Zastosowanie kryteriów D’Alemberta i Cauchy’ego do badania zbieżności szeregów.","odniesienie":["P_W01","P_U01"]},{"lp":7,"opis":"Szeregi funkcyjne – obliczanie granic ciągów funkcyjnych. Ocena zbieżności jednostajnej szeregów funkcyjnych przy pomocy tw. Weierstrassa.","odniesienie":["P_W01","P_U01"]},{"lp":8,"opis":"Rachunek całkowy – obliczanie całki nieoznaczonej metodą podstawiania i przez części.","odniesienie":["P_W01","P_U01"]},{"lp":9,"opis":"Obliczanie całki oznaczonej. Wykorzystanie całki oznaczonej do obliczania pól figur płaskich – obliczanie pola figury ograniczonej wykresami funkcji.","odniesienie":["P_W01","P_U01"]},{"lp":10,"opis":"Funkcje wielu zmiennych – wyznaczanie dziedziny funkcji dwóch zmiennych, graficzne przedstawienie.","odniesienie":["P_W01","P_U01"]},{"lp":11,"opis":"Rachunek różniczkowy wielu zmiennych – obliczanie pochodnych cząstkowych i zastosowanie do wyznaczania ekstremum lokalnego. Wyznaczanie wartości najmniejszej i największej funkcji na wybranych zbiorach.","odniesienie":["P_W01","P_U01"]},{"lp":12,"opis":"Elementy geometrii analitycznej – wyznaczanie równania prostej/płaszczyzny przechodzącej przez podane punkty, obliczanie odległości punktu od prostej oraz dwóch prostych równoległych, obliczanie pól wielokątów rozpiętych na podanych wektorach.","odniesienie":["P_W01","P_U01"]}]}',

  -- efekty_ksztalcenia_json:
  N'[
     {"lp":"P_W01","opis":"Student zna podstawowe pojęcia Algebry Liniowej i Analizy Matematycznej","odniesienie":["K_W01"],"rodzaj":"wiedza"},
     {"lp":"P_U01","opis":"Student potrafi rozwiązywać zadania z Algebry Liniowej i Analizy Matematycznej","odniesienie":["K_U06","K_U07"],"rodzaj":"umiejetnosci"},
     {"lp":"P_K01","opis":"Student odczuwa potrzebę zdobywania nowych kompetencji matematycznych","odniesienie":["K_K01","K_K02"],"rodzaj":"kompetencje spoleczne"}
   ]',
  -- metody_weryfikacji_json:
  N'[
     {"lp":"P_W01","opis":"Praca pisemna + ustny test wiedzy. Ocena zadań projektowych oraz obserwacja wykonania zadań praktycznych","forma_zajec":"Ćwiczenia","rodzaj":"wiedza"},
     {"lp":"P_U01","opis":"Obserwacja i ocena wykonania zadań praktycznego, Ocena zadań projektowych oraz obserwacja wykonania zadań praktycznych ","forma_zajec":"Ćwiczenia","rodzaj":"umiejetnosci"},
     {"lp":"P_K01","opis":"Obserwacja i ocena wykonania zadań praktycznego","forma_zajec":"Ćwiczenia","rodzaj":"kompetencje_spoleczne"}
   ]',
  -- kryteria_oceny_json:
  N'[
     {"lp":"P_W01","ocena2":"Nie zna podstawowych pojęć z Algebry Liniowej lub/i Analizy Matematycznej","ocena3":"Zna na podstawowym poziomie pojęcia z Algebry Liniowej lub/i Analizy Matematycznej. ","ocena4":"Zna na dobrym poziomie pojęcia z Algebry Liniowej i Analizy przykładowe zastosowania","ocena5":"Zna pojęcia z Algebry i Analizy, umie znaleźć związki pomiędzy nimi i pokazać ich zastosowania"},
     {"lp":"P_U01","ocena2":"Nie umie podać rozwiązywania podstawowych zadań z Algebry Liniowej i Analizy Matematycznej","ocena3":"Umie wykorzystać nabytą wiedzę do rozwiązywania podstawowych zadań z Algebry Liniowej i Analizy Matematycznej.","ocena4":"Umie wykorzystać nabytą wiedzę do rozwiązywania większości wymaganych zadań z Algebry i Analizy.","ocena5":"Umie wykorzystać nabytą wiedzę do rozwiązywania wymaganych zadań z  Algebry i Analizy. Umie zinterpretować otrzymane wyniki."},
     {"lp":"P_K01","ocena2":"Nie wykazuje chęci nabywania nowych kompetencji matematycznych","ocena3":"Dąży do zdobywania nowych kompetencji matematycznych","ocena4":"Dąży do zdobywania nowych kompetencji matematycznych","ocena5":"Dąży do zdobywania nowych kompetencji matematycznych"}
   ]',

  -- naklad_pracy_json:
  N'{"zajecia_S":60,"zajecia_N":40,"konsultacje_S":6,"konsultacje_N":6,"projekt_S":0,"projekt_N":0,"sam_przygotowanie_S":57,"sam_przygotowanie_N":77,"zaliczenia_S":2,"zaliczenia_N":2,"sumaryczne_S":125,"sumaryczne_N":125,"kontakt_S":68,"kontakt_N":68,"praktyczne_S":30,"praktyczne_N":20,"zawodowe_S":10,"zawodowe_N":10,"badania_S":50,"badania_N":50}',


  -- literatura_json:
  N'{"podstawowa":["Kostrikin A.I., Wstęp do algebry, Cz. 1 i 2, PWN, Warszawa 2004.","Krysicki W., Włodarski L., Analiza matematyczna w zadaniach, PWN, Warszawa 2002.","Rudin W., Podstawy analizy matematycznej, PWN, Warszawa 1998."],
    "uzupelniajaca":["Białynicki-Birula A. ,Algebra liniowa z geometrią, PWN, Warszawa 1979.","Birkholc A., Analiza matematyczna. Funkcje wielu zmiennych, PWN, Warszawa 2002.","Dobrowolska K., Dyczka W., Jakuszenkow H., Matematyka dla studentów studiów technicznych, Wyd. 10, HELPMATH, Łódź 2003."],
    "inne":[]}',

 --metody_realizacji_json:
 N'{
     "wyklad_opis": "Wykład prowadzony metodą podającą wspomagany prezentacjami multimedialnymi.",
     "cwiczenia_opis": "Metoda problemowa, dyskusja, analiza i rozwiązywanie zadań",
     "inne_materialy_dydaktyczne": null
   }'
   
);
GO

INSERT INTO dbo.koordynatorzy_sylabusu (sylabus_id,uzytkownik_id) VALUES (1,1);
GO

