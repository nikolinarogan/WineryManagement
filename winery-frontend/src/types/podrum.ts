export interface Podrum {
  idpod: number
  temp: number
  nazivpod: string
  brojBuradi: number
}

export interface PodrumDetail {
  idpod: number
  temp: number
  nazivpod: string
  brojBuradi: number
}

export interface CreatePodrumRequest {
  temp: number
  nazivpod: string
}

export interface UpdatePodrumRequest {
  temp: number
  nazivpod: string
}

