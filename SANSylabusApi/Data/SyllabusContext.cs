using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SylabusAPI.Models;

namespace SylabusAPI.Data;

// Klasa kontekstu bazy danych – główny punkt komunikacji z bazą danych
public partial class SyllabusContext : DbContext
{
    // Konstruktor bez parametrów – używany np. przy projektowaniu w EF Tools
    public SyllabusContext()
    {
    }

    // Konstruktor z opcjami konfiguracyjnymi (np. przekazanymi przez DI)
    public SyllabusContext(DbContextOptions<SyllabusContext> options)
        : base(options)
    {
    }

    // DbSety odpowiadające tabelom w bazie danych
    public virtual DbSet<koordynatorzy_sylabusu> koordynatorzy_sylabusus { get; set; }
    public virtual DbSet<przedmioty> przedmioties { get; set; }
    public virtual DbSet<siatka_przedmiotow> siatka_przedmiotows { get; set; }
    public virtual DbSet<sylabus_historium> sylabus_historia { get; set; }
    public virtual DbSet<sylabusy> sylabusies { get; set; }
    public virtual DbSet<uzytkownicy> uzytkownicies { get; set; }

    // Konfiguracja połączenia z bazą danych (gdy nie została wcześniej ustawiona)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Domyślne połączenie – nazwa pochodzi z pliku appsettings.json
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    // Konfiguracja mapowania modeli do tabel i relacji
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // -------------------------
        // koordynatorzy_sylabusu
        // -------------------------
        modelBuilder.Entity<koordynatorzy_sylabusu>(entity =>
        {
            entity.HasKey(e => e.id); // Klucz główny

            entity.ToTable("koordynatorzy_sylabusu");

            // Relacja: 1 sylabus → wielu koordynatorów
            entity.HasOne(d => d.sylabus)
                .WithMany(p => p.koordynatorzy_sylabusus)
                .HasForeignKey(d => d.sylabus_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Relacja: 1 użytkownik → wielu przypisanych sylabusów
            entity.HasOne(d => d.uzytkownik)
                .WithMany(p => p.koordynatorzy_sylabusus)
                .HasForeignKey(d => d.uzytkownik_id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // -------------------------
        // przedmioty
        // -------------------------
        modelBuilder.Entity<przedmioty>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.ToTable("przedmioty");

            // Własności tekstowe z ograniczeniem długości
            entity.Property(e => e.kierunek).HasMaxLength(100);
            entity.Property(e => e.nazwa).HasMaxLength(255);
            entity.Property(e => e.osrodek).HasMaxLength(100);
            entity.Property(e => e.stopien).HasMaxLength(50);
        });

        // -------------------------
        // siatka_przedmiotow
        // -------------------------
        modelBuilder.Entity<siatka_przedmiotow>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.ToTable("siatka_przedmiotow");

            entity.Property(e => e.typ).HasMaxLength(20);

            // Relacja do tabeli przedmioty
            entity.HasOne(d => d.przedmiot)
                .WithMany(p => p.siatka_przedmiotows)
                .HasForeignKey(d => d.przedmiot_id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // -------------------------
        // sylabus_historium
        // -------------------------
        modelBuilder.Entity<sylabus_historium>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.Property(e => e.czas_zmiany).HasColumnType("datetime");
            entity.Property(e => e.wersja_wtedy).HasMaxLength(20);

            entity.HasOne(d => d.sylabus)
                .WithMany(p => p.sylabus_historia)
                .HasForeignKey(d => d.sylabus_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.zmieniony_przezNavigation)
                .WithMany(p => p.sylabus_historia)
                .HasForeignKey(d => d.zmieniony_przez)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // -------------------------
        // sylabusy
        // -------------------------
        modelBuilder.Entity<sylabusy>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.ToTable("sylabusy");

            // Pola tekstowe z limitami długości
            entity.Property(e => e.data_powstania).HasColumnType("datetime");
            entity.Property(e => e.nazwa_jednostki_organizacyjnej).HasMaxLength(255);
            entity.Property(e => e.nazwa_specjalnosci).HasMaxLength(255);
            entity.Property(e => e.profil_ksztalcenia).HasMaxLength(255);
            entity.Property(e => e.rodzaj_modulu_ksztalcenia).HasMaxLength(255);
            entity.Property(e => e.rok_data).HasMaxLength(20);
            entity.Property(e => e.wersja).HasMaxLength(20);

            // Relacja do użytkownika tworzącego sylabus
            entity.HasOne(d => d.kto_stworzylNavigation)
                .WithMany(p => p.sylabusies)
                .HasForeignKey(d => d.kto_stworzyl)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Relacja do przedmiotu
            entity.HasOne(d => d.przedmiot)
                .WithMany(p => p.sylabusies)
                .HasForeignKey(d => d.przedmiot_id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // -------------------------
        // uzytkownicy
        // -------------------------
        modelBuilder.Entity<uzytkownicy>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.ToTable("uzytkownicy");

            // Indeksy unikalne
            entity.HasIndex(e => e.login).IsUnique();
            entity.HasIndex(e => e.email).IsUnique();

            // Pola tekstowe
            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.haslo).HasMaxLength(255);
            entity.Property(e => e.imie_nazwisko).HasMaxLength(255);
            entity.Property(e => e.login).HasMaxLength(100);
            entity.Property(e => e.sol).HasMaxLength(24).IsUnicode(false);
            entity.Property(e => e.typ_konta).HasMaxLength(20);
            entity.Property(e => e.tytul).HasMaxLength(50);
        });

        // Opcjonalna metoda partial do rozszerzania konfiguracji (jeśli istnieje w innej części klasy)
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
