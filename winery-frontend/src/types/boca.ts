export interface Boca {
  idboce: number
  cena: number | null
  zapremina: number
  vinoIdvina: number | null
  nazivVina: string
  tipVina: string
  magacinIdmag: number | null
  nazivMagacina: string
}

export interface BocaInventar {
  idboce: number
  cena: number | null
  zapremina: number
  nazivVina: string
  tipVina: string
  nazivMagacina: string
}

export interface CreateBocaPunjenjeRequest {
  vinoIdvina: number
  magacinIdmag: number
  brojBoca: number
  zapremina: number
  cena: number | null
}

export interface BocePunjenjeResult {
  brojKreiranihBoca: number
  nazivVina: string
  nazivMagacina: string
  kreiraneBoceIds: number[]
}

