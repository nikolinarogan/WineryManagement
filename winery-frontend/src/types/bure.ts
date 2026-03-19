export interface Bure {
  idbur: number
  zapremina: number
  materijal: string
  oznakabur: string
  podrumIdpod: number | null
  podrumNaziv: string | null
}

export interface BureDetail {
  idbur: number
  zapremina: number
  materijal: string
  oznakabur: string
  podrumIdpod: number | null
  podrumNaziv: string | null
  brojLagerovanihVina: number
}

export interface CreateBureRequest {
  zapremina: number
  materijal: string
  oznakabur: string
  podrumIdpod: number
}

export interface UpdateBureRequest {
  zapremina: number
  materijal: string
  oznakabur: string
}

