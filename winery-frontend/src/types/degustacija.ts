export interface Degustacija {
  iddeg: number
  nazivdeg: string
  datdeg: string
  kapacitetdeg: number
  brojVina: number
}

export interface DegustacijaDetail {
  iddeg: number
  nazivdeg: string
  datdeg: string
  kapacitetdeg: number
  vina: VinoBasic[]
  somelijeri: SomleijerBasic[]
}

export interface VinoBasic {
  idvina: number
  nazivvina: string
  tipvina: string
}

export interface SomleijerBasic {
  idzap: number
  ime: string
  prezime: string
  specijalnost: string
}

export interface CreateDegustacijaRequest {
  nazivdeg: string
  datdeg: string
  kapacitetdeg: number
  vinaIds: number[]
  somleijerIdzap: number
}

export interface UpdateDegustacijaRequest {
  nazivdeg: string
  datdeg: string
  kapacitetdeg: number
  vinaIds: number[]
}

