

export interface Radovi {
  idrad: number
  pocrad: string
  zavrrad: string
  oprema: string
  brojParcela: number
  brojRadnika: number
}

export interface RadoviDetail {
  idrad: number
  pocrad: string
  zavrrad: string
  oprema: string
  parcele: ParcelaNaRadu[]
  radnici: RadnikNaRadu[]
}

export interface ParcelaNaRadu {
  idp: number
  nazivparcele: string
  vinogradNaziv: string
  povrsina: number
}

export interface RadnikNaRadu {
  radnikIdzap: number
  ime: string
  prezime: string
  email: string
}

export interface CreateRadoviRequest {
  pocrad: string
  zavrrad: string
  oprema: string
  parcelaIds: number[]
}

export interface UpdateRadoviRequest {
  pocrad: string
  zavrrad: string
  oprema: string
}

export interface AddRadnikToRadRequest {
  radnikIdzap: number
}

export interface RadnikRad {
  idrad: number
  pocrad: string
  zavrrad: string
  oprema: string
  parcele: ParcelaNaRadu[]
}

