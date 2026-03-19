export interface Vinograd {
  idv: number
  naziv: string
  datosn: string
  povrsina: number
  nadmorskavis: number
  tipzemljista: string
  navodnjavanje: string
  brojParcela: number
}

export interface VinogradDetail {
  idv: number
  naziv: string
  datosn: string
  povrsina: number
  nadmorskavis: number
  tipzemljista: string
  navodnjavanje: string
  parcele: Parcela[]
}

export interface Parcela {
  idp: number
  brojcokota: number
  povrsina: number
  nazivparcele: string
  vinogradIdv: number
  sortagrozdjaIdsrt?: number
  sortaNaziv?: string
}

export interface CreateVinogradRequest {
  naziv: string
  datosn: string
  povrsina: number
  nadmorskavis: number
  tipzemljista: string
  navodnjavanje: string
  parcele?: CreateParcelaRequest[]
}

export interface CreateParcelaRequest {
  brojcokota: number
  povrsina: number
  nazivparcele: string
  sortagrozdjaIdsrt?: number
}

export interface UpdateVinogradRequest {
  naziv: string
  datosn: string
  povrsina: number
  nadmorskavis: number
  tipzemljista: string
  navodnjavanje: string
}

export interface UpdateParcelaRequest {
  brojcokota: number
  povrsina: number
  nazivparcele: string
  sortagrozdjaIdsrt?: number
}

