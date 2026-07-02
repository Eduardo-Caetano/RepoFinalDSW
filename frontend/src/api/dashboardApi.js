import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5022/api',
})

export async function getDashboardFinanceiro(id) {
  const response = await api.get(`/deputados/${id}/dashboard-financeiro`)
  return response.data
}
