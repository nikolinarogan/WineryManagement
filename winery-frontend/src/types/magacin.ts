export interface Magacin {
  idmag: number
  nazivmag: string
  kapacitetmag: number
  tempmag: number
  brojBoca: number
  popunjenost: number
}

export interface MagacinDetail {
  idmag: number
  nazivmag: string
  kapacitetmag: number
  tempmag: number
  brojBoca: number
  popunjenost: number
}

export interface CreateMagacinRequest {
  nazivmag: string
  kapacitetmag: number
  tempmag: number
}

export interface UpdateMagacinRequest {
  nazivmag: string
  kapacitetmag: number
  tempmag: number
}

