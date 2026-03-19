import api from './api'
import type { 
  Berba, 
  BerbaDetail, 
  CreateBerbaRequest, 
  UpdateBerbaRequest,
  Rasporedbranja,
  RasporedbranjaDetail,
  CreateRasporedbranjaRequest,
  UpdateRasporedbranjaRequest,
  AddRadnikToRasporedRequest,
  RadnikRaspored,
  UpdateRadnikKolicinaRequest,
  BerbaStatistika
} from '@/types/berba'

class BerbaService {
  async getAllBerbe(): Promise<Berba[]> {
    const response = await api.get('/berba')
    return response.data
  }

  async getBerbaById(id: number): Promise<BerbaDetail> {
    const response = await api.get(`/berba/${id}`)
    return response.data
  }

  async createBerba(data: CreateBerbaRequest): Promise<Berba> {
    const response = await api.post('/berba', data)
    return response.data
  }

  async updateBerba(id: number, data: UpdateBerbaRequest): Promise<void> {
    await api.put(`/berba/${id}`, data)
  }

  async deleteBerba(id: number): Promise<void> {
    await api.delete(`/berba/${id}`)
  }

  async addParcelaToBerba(berbaId: number, parcelaId: number): Promise<void> {
    await api.post(`/berba/${berbaId}/parcele/${parcelaId}`)
  }

  async removeParcelaFromBerba(berbaId: number, parcelaId: number): Promise<void> {
    await api.delete(`/berba/${berbaId}/parcele/${parcelaId}`)
  }


  async getAllRasporedi(berbaId?: number): Promise<Rasporedbranja[]> {
    const params = berbaId ? { berbaId } : {}
    const response = await api.get('/berba/rasporedi', { params })
    return response.data
  }

  async getRasporedById(id: number): Promise<RasporedbranjaDetail> {
    const response = await api.get(`/berba/rasporedi/${id}`)
    return response.data
  }

  async createRaspored(data: CreateRasporedbranjaRequest): Promise<Rasporedbranja> {
    const response = await api.post('/berba/rasporedi', data)
    return response.data
  }

  async updateRaspored(id: number, data: UpdateRasporedbranjaRequest): Promise<void> {
    await api.put(`/berba/rasporedi/${id}`, data)
  }

  async deleteRaspored(id: number): Promise<void> {
    await api.delete(`/berba/rasporedi/${id}`)
  }

  async addRadnikToRaspored(rasporedId: number, data: AddRadnikToRasporedRequest): Promise<void> {
    await api.post(`/berba/rasporedi/${rasporedId}/radnici`, data)
  }

  async removeRadnikFromRaspored(rasporedId: number, radnikId: number): Promise<void> {
    await api.delete(`/berba/rasporedi/${rasporedId}/radnici/${radnikId}`)
  }

  async getMojiRasporedi(): Promise<Rasporedbranja[]> {
    const response = await api.get('/berba/moji-rasporedi')
    return response.data
  }

  async getMojiRasporediDetalji(): Promise<RadnikRaspored[]> {
    const response = await api.get('/berba/moji-rasporedi/detalji')
    return response.data
  }

  async updateMojaKolicina(rasporedId: number, data: UpdateRadnikKolicinaRequest): Promise<void> {
    await api.put(`/berba/moji-rasporedi/${rasporedId}/kolicina`, data)
  }

  async getBerbaStatistika(berbaId: number): Promise<BerbaStatistika> {
    const response = await api.get(`/berba/${berbaId}/statistika`)
    return response.data
  }
}

export default new BerbaService()

