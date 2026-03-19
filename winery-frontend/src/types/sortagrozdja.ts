export interface Sortagrozdja {
  idsrt: number
  nazivsorte: string
  bojasorte: string
  porijeklosorte: string
  periodsazr: string
  kiselost: number
  brojParcela: number
}

export interface SortagrozdjaDetail {
  idsrt: number
  nazivsorte: string
  bojasorte: string
  porijeklosorte: string
  periodsazr: string
  kiselost: number
  parcele: ParcelaInfo[]
}

export interface ParcelaInfo {
  idp: number
  nazivparcele: string
  povrsina: number
  brojcokota: number
  vinogradNaziv: string
  vinogradIdv: number
}

export interface CreateSortagrozdjaRequest {
  nazivsorte: string
  bojasorte: string
  porijeklosorte: string
  periodsazr: string
  kiselost: number
}

export interface UpdateSortagrozdjaRequest {
  nazivsorte: string
  bojasorte: string
  porijeklosorte: string
  periodsazr: string
  kiselost: number
}

