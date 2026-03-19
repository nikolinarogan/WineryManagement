

export interface UbranasirovinaZaTretman {
  idubrsir: number
  kolicina: number
  datprijema: string
  berbaNaziv: string
  sezona: number
  parcelaNaziv: string
  vinogradNaziv: string
  sortaNaziv: string | null
  status: string // Nova/UTretmanu/SpremaZaVino/Obradjena
  brojTretmana: number
  aktivniTretmani: number
  postojiSirovovino: boolean
}

export interface Tretman {
  idtretmana: number
  naziv: string
  datpocetkatret: string
  datzavresetkatret: string | null
  trajanjeUDanima: number
  jeAktivan: boolean
  ubranasirovinaIdubrsir: number
  brojSirovina: number
  enolozi: EnologTretmana[]
}

export interface EnologTretmana {
  idzap: number
  ime: string
  prez: string
}

export interface TretmanDetail {
  idtretmana: number
  naziv: string
  datpocetkatret: string
  datzavresetkatret: string | null
  trajanjeUDanima: number
  jeAktivan: boolean
  ubranaSirovina: UbranasirovinaInfo
  sirovine: SirovinaUTretmanu[]
  enolozi: EnologTretmana[]
}

export interface UbranasirovinaInfo {
  idubrsir: number
  kolicina: number
  berbaNaziv: string
  parcelaNaziv: string
  sortaNaziv: string | null
}

export interface SirovinaUTretmanu {
  sirovinazatretmanIdsir: number
  nazivSirovine: string
  kolicina: number
}

export interface CreateTretmanRequest {
  naziv: string
  datpocetkatret: string
  ubranasirovinaIdubrsir: number
}

export interface CloseTretmanRequest {
  datzavresetkatret: string
}

export interface AddSirovinaToTretmanRequest {
  sirovinazatretmanIdsir: number
  kolicina: number
}

