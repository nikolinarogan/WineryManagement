export interface Berba {
  idber: number
  nazber: string
  sezona: number
  brojParcela: number
}

export interface BerbaDetail {
  idber: number
  nazber: string
  sezona: number
  parcele: ParcelaBerba[]
}

export interface ParcelaBerba {
  parcelaIdp: number
  nazivparcele: string
  vinogradNaziv: string
  sortaNaziv?: string
  povrsina: number
  brojcokota: number
}

export interface CreateBerbaRequest {
  nazber: string
  sezona: number
  parcelaIds: number[]
}

export interface UpdateBerbaRequest {
  nazber: string
  sezona: number
}

// RasporedBranja types
export interface Rasporedbranja {
  idras: number
  pocbranja: string
  zavrsetakbranja: string
  ocekivaniprinos: number
  menadzerIdzap: number
  menadzerIme: string
  menadzerPrezime: string
  seodrzavaIdber: number
  berbaNaziv: string
  seodrzavaIdp: number
  parcelaNaziv: string
  brojRadnika: number
  brojRadnikaSaUnosom: number
  ukupnoUbrano: number
}

export interface RasporedbranjaDetail {
  idras: number
  pocbranja: string
  zavrsetakbranja: string
  ocekivaniprinos: number
  menadzerIdzap: number
  menadzerIme: string
  menadzerPrezime: string
  seodrzavaIdber: number
  berbaNaziv: string
  seodrzavaIdp: number
  parcelaNaziv: string
  vinogradNaziv: string
  radnici: RadnikNaRaspored[]
}

export interface RadnikNaRaspored {
  radnikIdzap: number
  ime: string
  prezime: string
  kolicinaubrgr: number
  datumbranja: string
}

export interface CreateRasporedbranjaRequest {
  pocbranja: string
  zavrsetakbranja: string
  ocekivaniprinos: number
  menadzerIdzap: number
  berbaIdber: number
  parcelaIdp: number
}

export interface UpdateRasporedbranjaRequest {
  pocbranja: string
  zavrsetakbranja: string
  ocekivaniprinos: number
}

export interface AddRadnikToRasporedRequest {
  radnikIdzap: number
  kolicinaubrgr: number
  datumbranja: string
}

export interface UpdateRadnikKolicinaRequest {
  kolicinaubrgr: number
  datumbranja: string
}

export interface RadnikRaspored {
  idras: number
  berbaNaziv: string
  parcelaNaziv: string
  vinogradNaziv: string
  pocbranja: string
  zavrsetakbranja: string
  mojaKolicina: number
  mojDatumBranja?: string
  menadzerIme: string
  menadzerPrezime: string
}

export interface BerbaStatistika {
  berbaIdber: number
  berbaNaziv: string
  sezona: number
  ukupnoUbrano: number
  ocekivanoUkupno: number
  procenatRealizacije: number
  brojRadnika: number
  brojRasporeda: number
  radniciUcinak: RadnikUcinak[]
}

export interface RadnikUcinak {
  radnikId: number
  radnikIme: string
  radnikPrezime: string
  kolicina: number
  datumBranja?: string
  parcelaNaziv: string
}

