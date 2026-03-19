// DTO za prikaz prijema
export interface Ubranasirovina {
  idubrsir: number
  kolicina: number
  datprijema: string
  rasporedbranjaIdras: number
  berbaNaziv: string
  parcelaNaziv: string
  vinogradNaziv: string
  sortaNaziv: string | null
}

// DTO za kreiranje prijema
export interface CreateUbranasirovinaRequest {
  kolicina: number
  datprijema: string
  rasporedbranjaIdras: number
}

// DTO za ažuriranje prijema
export interface UpdateUbranasirovinaRequest {
  kolicina: number
  datprijema: string
}

// DTO za raspored spreman za prijem
export interface RasporedZaPrijem {
  rasporedId: number
  berbaNaziv: string
  sezona: number
  parcelaNaziv: string
  vinogradNaziv: string
  sortaNaziv: string | null
  pocetakBranja: string
  zavrsetakBranja: string
  ocekivaniPrinos: number
  ukupnoUbrano: number
  procenatRealizacije: number
  ukupnoRadnika: number
  radnikaSaUnosom: number
}

// DTO za detaljni pregled prije prijema
export interface PrijemDetails {
  rasporedId: number
  berbaNaziv: string
  parcelaNaziv: string
  vinogradNaziv: string
  sortaNaziv: string | null
  pocetakBranja: string
  zavrsetakBranja: string
  ocekivaniPrinos: number
  ukupnoUbrano: number
  procenatRealizacije: number
  prosekPoRadniku: number
  standardnaDevijacija: number
  radnici: RadnikKolicina[]
  brojAnomalija: number
  anomalije: RadnikAnomalija[]
  preporucenaKolicina: number
  minValidnaKolicina: number
  maxValidnaKolicina: number
}

export interface RadnikKolicina {
  radnikId: number
  ime: string
  prezime: string
  kolicina: number
  datum: string
  odstupostotakOdProseka: number
  isOutlier: boolean
}

export interface RadnikAnomalija {
  radnikId: number
  ime: string
  prezime: string
  kolicina: number
  prosek: number
  odstupostotakPostotak: number
  tip: string // IZNAD_PROSEKA, ISPOD_PROSEKA
  poruka: string
}

// DTO za rezultat validacije
export interface PrijemValidationResult {
  valid: boolean
  errors: string[]
  warnings: string[]
  ukupnoUbrano: number
  gubici: number
  postotakGubitaka: number
}

// Request za validaciju
export interface ValidatePrijemRequest {
  rasporedId: number
  kolicina: number
}

