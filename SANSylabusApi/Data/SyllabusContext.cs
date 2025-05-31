using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SylabusAPI.Models;

namespace SylabusAPI.Data;

public partial class SyllabusContext : DbContext
{
    public SyllabusContext()
    {
    }

    public SyllabusContext(DbContextOptions<SyllabusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<koordynatorzy_sylabusu> koordynatorzy_sylabusus { get; set; }

    public virtual DbSet<przedmioty> przedmioties { get; set; }

    public virtual DbSet<siatka_przedmiotow> siatka_przedmiotows { get; set; }

    public virtual DbSet<sylabus_historium> sylabus_historia { get; set; }

    public virtual DbSet<sylabusy> sylabusies { get; set; }

    public virtual DbSet<uzytkownicy> uzytkownicies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<koordynatorzy_sylabusu>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__koordyna__3213E83FBA91E97D");

            entity.ToTable("koordynatorzy_sylabusu");

            entity.HasOne(d => d.sylabus).WithMany(p => p.koordynatorzy_sylabusus)
                .HasForeignKey(d => d.sylabus_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_koord_syl_sylabus");

            entity.HasOne(d => d.uzytkownik).WithMany(p => p.koordynatorzy_sylabusus)
                .HasForeignKey(d => d.uzytkownik_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_koord_syl_uzytkownicy");
        });

        modelBuilder.Entity<przedmioty>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__przedmio__3213E83F377595FB");

            entity.ToTable("przedmioty");

            entity.Property(e => e.kierunek).HasMaxLength(100);
            entity.Property(e => e.nazwa).HasMaxLength(255);
            entity.Property(e => e.osrodek).HasMaxLength(100);
            entity.Property(e => e.stopien).HasMaxLength(50);
        });

        modelBuilder.Entity<siatka_przedmiotow>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__siatka_p__3213E83F341D029C");

            entity.ToTable("siatka_przedmiotow");

            entity.Property(e => e.typ).HasMaxLength(20);

            entity.HasOne(d => d.przedmiot).WithMany(p => p.siatka_przedmiotows)
                .HasForeignKey(d => d.przedmiot_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_siatka_przedmiotow_przedmioty");
        });

        modelBuilder.Entity<sylabus_historium>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__sylabus___3213E83FA750B5CC");

            entity.Property(e => e.czas_zmiany).HasColumnType("datetime");
            entity.Property(e => e.wersja_wtedy).HasMaxLength(20);

            entity.HasOne(d => d.sylabus).WithMany(p => p.sylabus_historia)
                .HasForeignKey(d => d.sylabus_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_historia_sylabus");

            entity.HasOne(d => d.zmieniony_przezNavigation).WithMany(p => p.sylabus_historia)
                .HasForeignKey(d => d.zmieniony_przez)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_historia_uzytkownicy");
        });

        modelBuilder.Entity<sylabusy>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__sylabusy__3213E83FB11405E6");

            entity.ToTable("sylabusy");

            entity.Property(e => e.data_powstania).HasColumnType("datetime");
            entity.Property(e => e.nazwa_jednostki_organizacyjnej).HasMaxLength(255);
            entity.Property(e => e.nazwa_specjalnosci).HasMaxLength(255);
            entity.Property(e => e.profil_ksztalcenia).HasMaxLength(100);
            entity.Property(e => e.rodzaj_modulu_ksztalcenia).HasMaxLength(100);
            entity.Property(e => e.rok_data).HasMaxLength(20);
            entity.Property(e => e.wersja).HasMaxLength(20);

            entity.HasOne(d => d.kto_stworzylNavigation).WithMany(p => p.sylabusies)
                .HasForeignKey(d => d.kto_stworzyl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sylabusy_kto_stworzyl");

            entity.HasOne(d => d.przedmiot).WithMany(p => p.sylabusies)
                .HasForeignKey(d => d.przedmiot_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sylabusy_przedmioty");
        });

        modelBuilder.Entity<uzytkownicy>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__uzytkown__3213E83FA6416F21");

            entity.ToTable("uzytkownicy");

            entity.HasIndex(e => e.login, "UQ__uzytkown__7838F27223C243F6").IsUnique();

            entity.HasIndex(e => e.email, "UQ__uzytkown__AB6E61643A6B1E29").IsUnique();

            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.haslo).HasMaxLength(255);
            entity.Property(e => e.imie_nazwisko).HasMaxLength(255);
            entity.Property(e => e.login).HasMaxLength(100);
            entity.Property(e => e.sol)
                .HasMaxLength(24)
                .IsUnicode(false);
            entity.Property(e => e.typ_konta).HasMaxLength(20);
            entity.Property(e => e.tytul).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
