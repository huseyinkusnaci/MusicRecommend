using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MusicRecommend.Models
{
    public class Methods
    {
      public  static IList<int> KOrtalamalar(double[][] veri, int kumeSayisi)
        {
            var random = new Random(5555);
            // Her satırı rastgele bir kümeye ata
            var sonucKumesi = Enumerable
                                    .Range(0, veri.Length)
                                    .Select(index => (AtananKume: random.Next(0, kumeSayisi),
                                                  Degerler: veri[index]))
                                    .ToList();

            var boyutSayisi = veri[0].Length;
            var limit = 10000;
            var guncellendiMi = true;
            while (--limit > 0)
            {
                // kümelerin merkez noktalarını hesapla
                var merkezNoktalar = Enumerable.Range(0, kumeSayisi)
                                                .AsParallel()
                                                .Select(kumeNumarasi =>
                                                (
                                                kume: kumeNumarasi,
                                                merkezNokta: Enumerable.Range(0, boyutSayisi)
                                                                                    .Select(eksen => sonucKumesi.Where(s => s.AtananKume == kumeNumarasi)
                                                                                    .Average(s => s.Degerler[eksen]))
                                                                                    .ToArray())
                                                        ).ToArray();
                // Sonuç kümesini merkeze en yakın ile güncelle
                guncellendiMi = false;
                //for (int i = 0; i < sonucKumesi.Count; i++)
                Parallel.For(0, sonucKumesi.Count, i =>
                {
                    var satir = sonucKumesi[i];
                    var eskiAtananKume = satir.AtananKume;

                    var yeniAtananKume = merkezNoktalar.Select(n => (KumeNumarasi: n.kume,
                                                                    Uzaklik: UzaklikHesapla(satir.Degerler, n.merkezNokta)))
                                         .OrderBy(x => x.Uzaklik)
                                         .First()
                                         .KumeNumarasi;

                    if (yeniAtananKume != eskiAtananKume)
                    {
                        sonucKumesi[i] = (AtananKume: yeniAtananKume, Degerler: satir.Degerler);
                        guncellendiMi = true;
                    }
                });

                if (!guncellendiMi)
                {
                    break;
                }
            } // while

            return sonucKumesi.Select(k => k.AtananKume).ToArray();
        }

        public static double UzaklikHesapla(double[] birinciNokta, double[] ikinciNokta)
        {
            var kareliUzaklik = birinciNokta
                                    .Zip(ikinciNokta,
                                        (n1, n2) => Math.Pow(n1 - n2, 2)).Sum();
            return Math.Sqrt(kareliUzaklik);
        }


    }
}