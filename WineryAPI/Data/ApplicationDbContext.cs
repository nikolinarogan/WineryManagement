using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WineryAPI.Models;

namespace WineryAPI.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Berba> Berbas { get; set; }

    public virtual DbSet<Boca> Bocas { get; set; }

    public virtual DbSet<Bure> Bures { get; set; }

    public virtual DbSet<Degustacija> Degustacijas { get; set; }

    public virtual DbSet<Enolog> Enologs { get; set; }

    public virtual DbSet<JeAngazovan> JeAngazovans { get; set; }

    public virtual DbSet<JeOsnova> JeOsnovas { get; set; }

    public virtual DbSet<Magacin> Magacins { get; set; }

    public virtual DbSet<Menadzer> Menadzers { get; set; }

    public virtual DbSet<Parcela> Parcelas { get; set; }

    public virtual DbSet<Podrum> Podrums { get; set; }

    public virtual DbSet<Radnik> Radniks { get; set; }

    public virtual DbSet<Radovi> Radovis { get; set; }

    public virtual DbSet<Rasporedbranja> Rasporedbranjas { get; set; }

    public virtual DbSet<SeDodaje> SeDodajes { get; set; }

    public virtual DbSet<SeLageruje> SeLagerujes { get; set; }

    public virtual DbSet<Seodrzava> Seodrzavas { get; set; }

    public virtual DbSet<Sirovinazatretman> Sirovinazatretmen { get; set; }

    public virtual DbSet<Sirovovino> Sirovovinos { get; set; }

    public virtual DbSet<Somleijer> Somleijers { get; set; }

    public virtual DbSet<Sortagrozdja> Sortagrozdjas { get; set; }

    public virtual DbSet<Tretman> Tretmen { get; set; }

    public virtual DbSet<Ubranasirovina> Ubranasirovinas { get; set; }

    public virtual DbSet<Ucestvuje> Ucestvujes { get; set; }

    public virtual DbSet<Vino> Vinos { get; set; }

    public virtual DbSet<Vinograd> Vinograds { get; set; }

    public virtual DbSet<Zaposleni> Zaposlenis { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Berba>(entity =>
        {
            entity.HasKey(e => e.Idber).HasName("berba_pkey");

            entity.ToTable("berba");

            entity.Property(e => e.Idber).HasColumnName("idber");
            entity.Property(e => e.Nazber)
                .HasMaxLength(100)
                .HasColumnName("nazber");
            entity.Property(e => e.Sezona).HasColumnName("sezona");
        });

        modelBuilder.Entity<Boca>(entity =>
        {
            entity.HasKey(e => e.Idboce).HasName("boca_pkey");

            entity.ToTable("boca");

            entity.HasIndex(e => e.MagacinIdmag, "idx_boca_magacin");

            entity.HasIndex(e => e.VinoIdvina, "idx_boca_vino");

            entity.Property(e => e.Idboce).HasColumnName("idboce");
            entity.Property(e => e.Cena).HasColumnName("cena");
            entity.Property(e => e.MagacinIdmag).HasColumnName("magacin_idmag");
            entity.Property(e => e.VinoIdvina).HasColumnName("vino_idvina");
            entity.Property(e => e.Zapremina).HasColumnName("zapremina");

            entity.HasOne(d => d.MagacinIdmagNavigation).WithMany(p => p.Bocas)
                .HasForeignKey(d => d.MagacinIdmag)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_boca_magacin");

            entity.HasOne(d => d.VinoIdvinaNavigation).WithMany(p => p.Bocas)
                .HasForeignKey(d => d.VinoIdvina)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_boca_vino");
        });

        modelBuilder.Entity<Bure>(entity =>
        {
            entity.HasKey(e => e.Idbur).HasName("bure_pkey");

            entity.ToTable("bure");

            entity.HasIndex(e => e.PodrumIdpod, "idx_bure_podrum");

            entity.Property(e => e.Idbur).HasColumnName("idbur");
            entity.Property(e => e.Materijal)
                .HasMaxLength(50)
                .HasColumnName("materijal");
            entity.Property(e => e.Oznakabur)
                .HasMaxLength(50)
                .HasColumnName("oznakabur");
            entity.Property(e => e.PodrumIdpod).HasColumnName("podrum_idpod");
            entity.Property(e => e.Zapremina)
                .HasPrecision(7, 2)
                .HasColumnName("zapremina");

            entity.HasOne(d => d.PodrumIdpodNavigation).WithMany(p => p.Bures)
                .HasForeignKey(d => d.PodrumIdpod)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_bure_podrum");
        });

        modelBuilder.Entity<Degustacija>(entity =>
        {
            entity.HasKey(e => e.Iddeg).HasName("degustacija_pkey");

            entity.ToTable("degustacija");

            entity.Property(e => e.Iddeg).HasColumnName("iddeg");
            entity.Property(e => e.Datdeg).HasColumnName("datdeg");
            entity.Property(e => e.Kapacitetdeg).HasColumnName("kapacitetdeg");
            entity.Property(e => e.Nazivdeg)
                .HasMaxLength(150)
                .HasColumnName("nazivdeg");
        });

        modelBuilder.Entity<Enolog>(entity =>
        {
            entity.HasKey(e => e.Idzap).HasName("enolog_pkey");

            entity.ToTable("enolog");

            entity.Property(e => e.Idzap)
                .ValueGeneratedNever()
                .HasColumnName("idzap");
            entity.Property(e => e.Brsert)
                .HasMaxLength(50)
                .HasColumnName("brsert");

            entity.HasOne(d => d.IdzapNavigation).WithOne(p => p.Enolog)
                .HasForeignKey<Enolog>(d => d.Idzap)
                .HasConstraintName("fk_enolog_zaposleni");

            entity.HasMany(d => d.TretmanIdtretmanas).WithMany(p => p.EnologIdzaps)
                .UsingEntity<Dictionary<string, object>>(
                    "Obavlja",
                    r => r.HasOne<Tretman>().WithMany()
                        .HasForeignKey("TretmanIdtretmana")
                        .HasConstraintName("fk_obavlja_tretman"),
                    l => l.HasOne<Enolog>().WithMany()
                        .HasForeignKey("EnologIdzap")
                        .HasConstraintName("fk_obavlja_enolog"),
                    j =>
                    {
                        j.HasKey("EnologIdzap", "TretmanIdtretmana").HasName("obavlja_pkey");
                        j.ToTable("obavlja");
                        j.IndexerProperty<int>("EnologIdzap").HasColumnName("enolog_idzap");
                        j.IndexerProperty<int>("TretmanIdtretmana").HasColumnName("tretman_idtretmana");
                    });
        });

        modelBuilder.Entity<JeAngazovan>(entity =>
        {
            entity.HasKey(e => new { e.RasporedbranjaIdras, e.RadnikIdzap }).HasName("je_angazovan_pkey");

            entity.ToTable("je_angazovan");

            entity.Property(e => e.RasporedbranjaIdras).HasColumnName("rasporedbranja_idras");
            entity.Property(e => e.RadnikIdzap).HasColumnName("radnik_idzap");
            entity.Property(e => e.Datumbranja).HasColumnName("datumbranja");
            entity.Property(e => e.Kolicinaubrgr)
                .HasPrecision(4, 2)
                .HasColumnName("kolicinaubrgr");

            entity.HasOne(d => d.RadnikIdzapNavigation).WithMany(p => p.JeAngazovans)
                .HasForeignKey(d => d.RadnikIdzap)
                .HasConstraintName("fk_jeangazovan_radnik");

            entity.HasOne(d => d.RasporedbranjaIdrasNavigation).WithMany(p => p.JeAngazovans)
                .HasForeignKey(d => d.RasporedbranjaIdras)
                .HasConstraintName("fk_jeangazovan_raspored");
        });

        modelBuilder.Entity<JeOsnova>(entity =>
        {
            entity.HasKey(e => new { e.UbranasirovinaIdubrsir, e.SirovovinoIdsirvina })
                .HasName("je_osnova_pkey");

            entity.ToTable("je_osnova");

            entity.Property(e => e.UbranasirovinaIdubrsir)
                .HasColumnName("ubranasirovina_idubrsir");

            entity.Property(e => e.SirovovinoIdsirvina)
                .HasColumnName("sirovovino_idsirvina");

            entity.Property(e => e.KolicinaGrozdja)
                .HasPrecision(7, 2)
                .HasColumnName("kolicina_grozdja");

            entity.HasOne(d => d.UbranasirovinaIdubrsirNavigation)
                .WithMany(p => p.JeOsnovas)
                .HasForeignKey(d => d.UbranasirovinaIdubrsir)
                .HasConstraintName("fk_jeosnova_ubranasirovina");

            entity.HasOne(d => d.SirovovinoIdsirvinaNavigation)
                .WithMany(p => p.JeOsnovas)
                .HasForeignKey(d => d.SirovovinoIdsirvina)
                .HasConstraintName("fk_jeosnova_sirovovino");
        });

        modelBuilder.Entity<Magacin>(entity =>
        {
            entity.HasKey(e => e.Idmag).HasName("magacin_pkey");

            entity.ToTable("magacin");

            entity.Property(e => e.Idmag).HasColumnName("idmag");
            entity.Property(e => e.Kapacitetmag).HasColumnName("kapacitetmag");
            entity.Property(e => e.Nazivmag)
                .HasMaxLength(100)
                .HasColumnName("nazivmag");
            entity.Property(e => e.Tempmag)
                .HasPrecision(4, 2)
                .HasColumnName("tempmag");
        });

        modelBuilder.Entity<Menadzer>(entity =>
        {
            entity.HasKey(e => e.Idzap).HasName("menadzer_pkey");

            entity.ToTable("menadzer");

            entity.Property(e => e.Idzap)
                .ValueGeneratedNever()
                .HasColumnName("idzap");
            entity.Property(e => e.Bonusucinak)
                .HasPrecision(4, 2)
                .HasColumnName("bonusucinak");

            entity.HasOne(d => d.IdzapNavigation).WithOne(p => p.Menadzer)
                .HasForeignKey<Menadzer>(d => d.Idzap)
                .HasConstraintName("fk_menadzer_zaposleni");
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Idp).HasName("parcela_pkey");

            entity.ToTable("parcela");

            entity.HasIndex(e => e.SortagrozdjaIdsrt, "idx_parcela_sorta");

            entity.HasIndex(e => e.VinogradIdv, "idx_parcela_vinograd");

            entity.Property(e => e.Idp).HasColumnName("idp");
            entity.Property(e => e.Brojcokota).HasColumnName("brojcokota");
            entity.Property(e => e.Nazivparcele)
                .HasMaxLength(100)
                .HasColumnName("nazivparcele");
            entity.Property(e => e.Povrsina)
                .HasPrecision(7, 2)
                .HasColumnName("povrsina");
            entity.Property(e => e.SortagrozdjaIdsrt).HasColumnName("sortagrozdja_idsrt");
            entity.Property(e => e.VinogradIdv).HasColumnName("vinograd_idv");

            entity.HasOne(d => d.SortagrozdjaIdsrtNavigation).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.SortagrozdjaIdsrt)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_parcela_sorta");

            entity.HasOne(d => d.VinogradIdvNavigation).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.VinogradIdv)
                .HasConstraintName("fk_parcela_vinograd");

            entity.HasMany(d => d.RadoviIdrads).WithMany(p => p.ParcelaIdps)
                .UsingEntity<Dictionary<string, object>>(
                    "SeOrganizuju",
                    r => r.HasOne<Radovi>().WithMany()
                        .HasForeignKey("RadoviIdrad")
                        .HasConstraintName("fk_seorganizuju_radovi"),
                    l => l.HasOne<Parcela>().WithMany()
                        .HasForeignKey("ParcelaIdp")
                        .HasConstraintName("fk_seorganizuju_parcela"),
                    j =>
                    {
                        j.HasKey("ParcelaIdp", "RadoviIdrad").HasName("se_organizuju_pkey");
                        j.ToTable("se_organizuju");
                        j.IndexerProperty<int>("ParcelaIdp").HasColumnName("parcela_idp");
                        j.IndexerProperty<int>("RadoviIdrad").HasColumnName("radovi_idrad");
                    });
        });

        modelBuilder.Entity<Podrum>(entity =>
        {
            entity.HasKey(e => e.Idpod).HasName("podrum_pkey");

            entity.ToTable("podrum");

            entity.Property(e => e.Idpod).HasColumnName("idpod");
            entity.Property(e => e.Nazivpod)
                .HasMaxLength(100)
                .HasColumnName("nazivpod");
            entity.Property(e => e.Temp)
                .HasPrecision(4, 2)
                .HasColumnName("temp");
        });

        modelBuilder.Entity<Radnik>(entity =>
        {
            entity.HasKey(e => e.Idzap).HasName("radnik_pkey");

            entity.ToTable("radnik");

            entity.Property(e => e.Idzap)
                .ValueGeneratedNever()
                .HasColumnName("idzap");
            entity.Property(e => e.Fizickaspremnost)
                .HasMaxLength(30)
                .HasColumnName("fizickaspremnost");

            entity.HasOne(d => d.IdzapNavigation).WithOne(p => p.Radnik)
                .HasForeignKey<Radnik>(d => d.Idzap)
                .HasConstraintName("fk_radnik_zaposleni");

            entity.HasMany(d => d.RadoviIdrads).WithMany(p => p.RadnikIdzaps)
                .UsingEntity<Dictionary<string, object>>(
                    "Radi",
                    r => r.HasOne<Radovi>().WithMany()
                        .HasForeignKey("RadoviIdrad")
                        .HasConstraintName("fk_radi_radovi"),
                    l => l.HasOne<Radnik>().WithMany()
                        .HasForeignKey("RadnikIdzap")
                        .HasConstraintName("fk_radi_radnik"),
                    j =>
                    {
                        j.HasKey("RadnikIdzap", "RadoviIdrad").HasName("radi_pkey");
                        j.ToTable("radi");
                        j.IndexerProperty<int>("RadnikIdzap").HasColumnName("radnik_idzap");
                        j.IndexerProperty<int>("RadoviIdrad").HasColumnName("radovi_idrad");
                    });
        });

        modelBuilder.Entity<Radovi>(entity =>
        {
            entity.HasKey(e => e.Idrad).HasName("radovi_pkey");

            entity.ToTable("radovi");

            entity.Property(e => e.Idrad).HasColumnName("idrad");
            entity.Property(e => e.Oprema)
                .HasMaxLength(500)
                .HasColumnName("oprema");
            entity.Property(e => e.Pocrad).HasColumnName("pocrad");
            entity.Property(e => e.Zavrrad).HasColumnName("zavrrad");
        });

        modelBuilder.Entity<Rasporedbranja>(entity =>
        {
            entity.HasKey(e => e.Idras).HasName("rasporedbranja_pkey");

            entity.ToTable("rasporedbranja");

            entity.HasIndex(e => e.MenadzerIdzap, "idx_raspored_menadzer");

            entity.Property(e => e.Idras).HasColumnName("idras");
            entity.Property(e => e.MenadzerIdzap).HasColumnName("menadzer_idzap");
            entity.Property(e => e.Ocekivaniprinos)
                .HasPrecision(7, 2)
                .HasColumnName("ocekivaniprinos");
            entity.Property(e => e.Pocbranja).HasColumnName("pocbranja");
            entity.Property(e => e.SeodrzavaIdber).HasColumnName("seodrzava_idber");
            entity.Property(e => e.SeodrzavaIdp).HasColumnName("seodrzava_idp");
            entity.Property(e => e.Zavrsetakbranja).HasColumnName("zavrsetakbranja");

            entity.HasOne(d => d.MenadzerIdzapNavigation).WithMany(p => p.Rasporedbranjas)
                .HasForeignKey(d => d.MenadzerIdzap)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_raspored_menadzer");

            entity.HasOne(d => d.Seodrzava).WithMany(p => p.Rasporedbranjas)
                .HasForeignKey(d => new { d.SeodrzavaIdber, d.SeodrzavaIdp })
                .HasConstraintName("fk_raspored_seodrzava");
        });

        modelBuilder.Entity<SeDodaje>(entity =>
        {
            entity.HasKey(e => new { e.SirovinazatretmanIdsir, e.TretmanIdtretmana }).HasName("se_dodaje_pkey");

            entity.ToTable("se_dodaje");

            entity.Property(e => e.SirovinazatretmanIdsir).HasColumnName("sirovinazatretman_idsir");
            entity.Property(e => e.TretmanIdtretmana).HasColumnName("tretman_idtretmana");
            entity.Property(e => e.Dodatakolicina)
                .HasPrecision(4, 2)
                .HasColumnName("dodatakolicina");

            entity.HasOne(d => d.SirovinazatretmanIdsirNavigation).WithMany(p => p.SeDodajes)
                .HasForeignKey(d => d.SirovinazatretmanIdsir)
                .HasConstraintName("fk_sedodaje_sirovina");

            entity.HasOne(d => d.TretmanIdtretmanaNavigation).WithMany(p => p.SeDodajes)
                .HasForeignKey(d => d.TretmanIdtretmana)
                .HasConstraintName("fk_sedodaje_tretman");
        });

        modelBuilder.Entity<SeLageruje>(entity =>
        {
            entity.HasKey(e => new { e.SirovovinoIdsirvina, e.BureIdbur }).HasName("se_lageruje_pkey");

            entity.ToTable("se_lageruje");

            entity.Property(e => e.SirovovinoIdsirvina).HasColumnName("sirovovino_idsirvina");
            entity.Property(e => e.BureIdbur).HasColumnName("bure_idbur");
            entity.Property(e => e.Datpraznjenja).HasColumnName("datpraznjenja");
            entity.Property(e => e.Datpunjenja).HasColumnName("datpunjenja");

            entity.HasOne(d => d.BureIdburNavigation).WithMany(p => p.SeLagerujes)
                .HasForeignKey(d => d.BureIdbur)
                .HasConstraintName("fk_selageruje_bure");

            entity.HasOne(d => d.SirovovinoIdsirvinaNavigation).WithMany(p => p.SeLagerujes)
                .HasForeignKey(d => d.SirovovinoIdsirvina)
                .HasConstraintName("fk_selageruje_sirovovino");
        });

        modelBuilder.Entity<Seodrzava>(entity =>
        {
            entity.HasKey(e => new { e.BerbaIdber, e.ParcelaIdp }).HasName("seodrzava_pkey");

            entity.ToTable("seodrzava");

            entity.Property(e => e.BerbaIdber).HasColumnName("berba_idber");
            entity.Property(e => e.ParcelaIdp).HasColumnName("parcela_idp");

            entity.HasOne(d => d.BerbaIdberNavigation).WithMany(p => p.Seodrzavas)
                .HasForeignKey(d => d.BerbaIdber)
                .HasConstraintName("fk_seodrzava_berba");

            entity.HasOne(d => d.ParcelaIdpNavigation).WithMany(p => p.Seodrzavas)
                .HasForeignKey(d => d.ParcelaIdp)
                .HasConstraintName("fk_seodrzava_parcela");
        });

        modelBuilder.Entity<Sirovinazatretman>(entity =>
        {
            entity.HasKey(e => e.Idsir).HasName("sirovinazatretman_pkey");

            entity.ToTable("sirovinazatretman");

            entity.Property(e => e.Idsir).HasColumnName("idsir");
            entity.Property(e => e.Naziv)
                .HasMaxLength(100)
                .HasColumnName("naziv");
        });

        modelBuilder.Entity<Sirovovino>(entity =>
        {
            entity.HasKey(e => e.Idsirvina).HasName("sirovovino_pkey");

            entity.ToTable("sirovovino");

            entity.Property(e => e.Idsirvina).HasColumnName("idsirvina");
            entity.Property(e => e.Godproizvodnje).HasColumnName("godproizvodnje");
            entity.Property(e => e.Kolicinasirvina)
                .HasPrecision(7, 2)
                .HasColumnName("kolicinasirvina");
            entity.Property(e => e.Kvalitet)
                .HasMaxLength(20)
                .HasColumnName("kvalitet");
            entity.Property(e => e.Nazivsirvina)
                .HasMaxLength(100)
                .HasColumnName("nazivsirvina");

            entity.HasMany(d => d.VinoIdvinas).WithMany(p => p.SirovovinoIdsirvinas)
                .UsingEntity<Dictionary<string, object>>(
                    "JeRaspordjeno",
                    r => r.HasOne<Vino>().WithMany()
                        .HasForeignKey("VinoIdvina")
                        .HasConstraintName("fk_jeraspordjeno_vino"),
                    l => l.HasOne<Sirovovino>().WithMany()
                        .HasForeignKey("SirovovinoIdsirvina")
                        .HasConstraintName("fk_jeraspordjeno_sirovovino"),
                    j =>
                    {
                        j.HasKey("SirovovinoIdsirvina", "VinoIdvina").HasName("je_raspordjeno_pkey");
                        j.ToTable("je_raspordjeno");
                        j.IndexerProperty<int>("SirovovinoIdsirvina").HasColumnName("sirovovino_idsirvina");
                        j.IndexerProperty<int>("VinoIdvina").HasColumnName("vino_idvina");
                    });
        });

        modelBuilder.Entity<Somleijer>(entity =>
        {
            entity.HasKey(e => e.Idzap).HasName("somleijer_pkey");

            entity.ToTable("somleijer");

            entity.Property(e => e.Idzap)
                .ValueGeneratedNever()
                .HasColumnName("idzap");
            entity.Property(e => e.Specijalnost)
                .HasMaxLength(150)
                .HasColumnName("specijalnost");

            entity.HasOne(d => d.IdzapNavigation).WithOne(p => p.Somleijer)
                .HasForeignKey<Somleijer>(d => d.Idzap)
                .HasConstraintName("fk_somleijer_zaposleni");

            entity.HasMany(d => d.DegustacijaIddegs).WithMany(p => p.SomleijerIdzaps)
                .UsingEntity<Dictionary<string, object>>(
                    "Vodi",
                    r => r.HasOne<Degustacija>().WithMany()
                        .HasForeignKey("DegustacijaIddeg")
                        .HasConstraintName("fk_vodi_degustacija"),
                    l => l.HasOne<Somleijer>().WithMany()
                        .HasForeignKey("SomleijerIdzap")
                        .HasConstraintName("fk_vodi_somleijer"),
                    j =>
                    {
                        j.HasKey("SomleijerIdzap", "DegustacijaIddeg").HasName("vodi_pkey");
                        j.ToTable("vodi");
                        j.IndexerProperty<int>("SomleijerIdzap").HasColumnName("somleijer_idzap");
                        j.IndexerProperty<int>("DegustacijaIddeg").HasColumnName("degustacija_iddeg");
                    });
        });

        modelBuilder.Entity<Sortagrozdja>(entity =>
        {
            entity.HasKey(e => e.Idsrt).HasName("sortagrozdja_pkey");

            entity.ToTable("sortagrozdja");

            entity.Property(e => e.Idsrt).HasColumnName("idsrt");
            entity.Property(e => e.Bojasorte)
                .HasMaxLength(50)
                .HasColumnName("bojasorte");
            entity.Property(e => e.Kiselost)
                .HasPrecision(3, 2)
                .HasColumnName("kiselost");
            entity.Property(e => e.Nazivsorte)
                .HasMaxLength(100)
                .HasColumnName("nazivsorte");
            entity.Property(e => e.Periodsazr)
                .HasMaxLength(100)
                .HasColumnName("periodsazr");
            entity.Property(e => e.Porijeklosorte)
                .HasMaxLength(100)
                .HasColumnName("porijeklosorte");
        });

        modelBuilder.Entity<Tretman>(entity =>
        {
            entity.HasKey(e => e.Idtretmana).HasName("tretman_pkey");

            entity.ToTable("tretman");

            entity.HasIndex(e => e.UbranasirovinaIdubrsir, "idx_tretman_ubranasirovina");

            entity.Property(e => e.Idtretmana).HasColumnName("idtretmana");
            entity.Property(e => e.Datpocetkatret).HasColumnName("datpocetkatret");
            entity.Property(e => e.Datzavresetkatret).HasColumnName("datzavresetkatret");
            entity.Property(e => e.Naziv)
                .HasMaxLength(200)
                .HasColumnName("naziv");
            entity.Property(e => e.UbranasirovinaIdubrsir).HasColumnName("ubranasirovina_idubrsir");

            entity.HasOne(d => d.UbranasirovinaIdubrsirNavigation).WithMany(p => p.Tretmen)
                .HasForeignKey(d => d.UbranasirovinaIdubrsir)
                .HasConstraintName("fk_tretman_ubranasirovina");
        });

        modelBuilder.Entity<Ubranasirovina>(entity =>
        {
            entity.HasKey(e => e.Idubrsir).HasName("ubranasirovina_pkey");

            entity.ToTable("ubranasirovina");

            entity.HasIndex(e => e.RasporedbranjaIdras, "ubranasirovina_rasporedbranja_idras_key").IsUnique();

            entity.Property(e => e.Idubrsir).HasColumnName("idubrsir");
            entity.Property(e => e.Datprijema).HasColumnName("datprijema");
            entity.Property(e => e.Kolicina)
                .HasPrecision(7, 2)
                .HasColumnName("kolicina");
            entity.Property(e => e.RasporedbranjaIdras).HasColumnName("rasporedbranja_idras");

            entity.HasOne(d => d.RasporedbranjaIdrasNavigation).WithOne(p => p.Ubranasirovina)
                .HasForeignKey<Ubranasirovina>(d => d.RasporedbranjaIdras)
                .HasConstraintName("fk_ubranasirovina_raspored");

            // STARA implicitna Many-to-Many konfiguracija (zakomentarisano):
            // entity.HasMany(d => d.SirovovinoIdsirvinas).WithMany(p => p.UbranasirovinaIdubrsirs)
            //     .UsingEntity<Dictionary<string, object>>(...);
        });

        modelBuilder.Entity<Ucestvuje>(entity =>
        {
            entity.HasKey(e => new { e.SortagrozdjaIdsrt, e.VinoIdvina }).HasName("ucestvuje_pkey");

            entity.ToTable("ucestvuje");

            entity.Property(e => e.SortagrozdjaIdsrt).HasColumnName("sortagrozdja_idsrt");
            entity.Property(e => e.VinoIdvina).HasColumnName("vino_idvina");
            entity.Property(e => e.Procenat)
                .HasPrecision(4, 2)
                .HasColumnName("procenat");

            entity.HasOne(d => d.SortagrozdjaIdsrtNavigation).WithMany(p => p.Ucestvujes)
                .HasForeignKey(d => d.SortagrozdjaIdsrt)
                .HasConstraintName("fk_ucestvuje_sorta");

            entity.HasOne(d => d.VinoIdvinaNavigation).WithMany(p => p.Ucestvujes)
                .HasForeignKey(d => d.VinoIdvina)
                .HasConstraintName("fk_ucestvuje_vino");
        });

        modelBuilder.Entity<Vino>(entity =>
        {
            entity.HasKey(e => e.Idvina).HasName("vino_pkey");

            entity.ToTable("vino");

            entity.Property(e => e.Idvina).HasColumnName("idvina");
            entity.Property(e => e.Nazivvina)
                .HasMaxLength(100)
                .HasColumnName("nazivvina");
            entity.Property(e => e.Procalk)
                .HasPrecision(4, 2)
                .HasColumnName("procalk");
            entity.Property(e => e.Tipvina)
                .HasMaxLength(100)
                .HasColumnName("tipvina");

            entity.HasMany(d => d.DegustacijaIddegs).WithMany(p => p.VinoIdvinas)
                .UsingEntity<Dictionary<string, object>>(
                    "SePrezentuje",
                    r => r.HasOne<Degustacija>().WithMany()
                        .HasForeignKey("DegustacijaIddeg")
                        .HasConstraintName("fk_seprezentuje_degustacija"),
                    l => l.HasOne<Vino>().WithMany()
                        .HasForeignKey("VinoIdvina")
                        .HasConstraintName("fk_seprezentuje_vino"),
                    j =>
                    {
                        j.HasKey("VinoIdvina", "DegustacijaIddeg").HasName("se_prezentuje_pkey");
                        j.ToTable("se_prezentuje");
                        j.IndexerProperty<int>("VinoIdvina").HasColumnName("vino_idvina");
                        j.IndexerProperty<int>("DegustacijaIddeg").HasColumnName("degustacija_iddeg");
                    });
        });

        modelBuilder.Entity<Vinograd>(entity =>
        {
            entity.HasKey(e => e.Idv).HasName("vinograd_pkey");

            entity.ToTable("vinograd");

            entity.Property(e => e.Idv).HasColumnName("idv");
            entity.Property(e => e.Datosn).HasColumnName("datosn");
            entity.Property(e => e.Nadmorskavis).HasColumnName("nadmorskavis");
            entity.Property(e => e.Navodnjavanje)
                .HasMaxLength(1)
                .HasColumnName("navodnjavanje");
            entity.Property(e => e.Naziv)
                .HasMaxLength(150)
                .HasColumnName("naziv");
            entity.Property(e => e.Povrsina)
                .HasPrecision(7, 2)
                .HasColumnName("povrsina");
            entity.Property(e => e.Tipzemljista)
                .HasMaxLength(50)
                .HasColumnName("tipzemljista");
        });

        modelBuilder.Entity<Zaposleni>(entity =>
        {
            entity.HasKey(e => e.Idzap).HasName("zaposleni_pkey");

            entity.ToTable("zaposleni");

            entity.HasIndex(e => e.Kategorija, "idx_zaposleni_kategorija");

            entity.HasIndex(e => e.ZaposleniIdzap, "idx_zaposleni_superior");

            entity.HasIndex(e => e.Email, "zaposleni_email_key").IsUnique();

            entity.HasIndex(e => e.Jmbg, "zaposleni_jmbg_key").IsUnique();

            entity.Property(e => e.Idzap).HasColumnName("idzap");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Ime)
                .HasMaxLength(20)
                .HasColumnName("ime");
            entity.Property(e => e.Jmbg)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasColumnName("jmbg");
            entity.Property(e => e.Kategorija)
                .HasMaxLength(100)
                .HasColumnName("kategorija");
            entity.Property(e => e.Prez)
                .HasMaxLength(30)
                .HasColumnName("prez");
            entity.Property(e => e.Sifra)
                .HasMaxLength(255)
                .HasColumnName("sifra");
            entity.Property(e => e.ZaposleniIdzap).HasColumnName("zaposleni_idzap");

            entity.HasOne(d => d.ZaposleniIdzapNavigation).WithMany(p => p.InverseZaposleniIdzapNavigation)
                .HasForeignKey(d => d.ZaposleniIdzap)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_zaposleni_superior");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
