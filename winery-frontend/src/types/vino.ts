export interface Vino {
  idvina: number
  nazivvina: string
  procalk: number
  tipvina: string
  brojSirovihVina: number
  sirovaVina: SirovoVinoUVinu[]
}

export interface SirovoVinoUVinu {
  idsirvina: number
  nazivsirvina: string
  kolicinasirvina: number
  kvalitet: string
  godproizvodnje: number
}

export interface VinoDetail {
  idvina: number
  nazivvina: string
  procalk: number
  tipvina: string
  sirovaVina: SirovoVinoUVinu[]
}

export interface CreateVinoRequest {
  nazivvina: string
  procalk: number
  tipvina: string
  sirovaVinaIds: number[]
}

export interface UpdateVinoRequest {
  nazivvina: string
  procalk: number
  tipvina: string
}

