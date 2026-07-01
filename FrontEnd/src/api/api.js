import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7092/api",
  headers: { "Content-Type": "application/json" },
});

export const getApiError = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  error?.message ||
  "Something went wrong";

export const createCrudApi = (resource) => ({
  getAll: (params = {}) => api.get(`/${resource}`, { params }).then((r) => r.data),
  getById: (id) => api.get(`/${resource}/${id}`).then((r) => r.data),
  create: (payload) => api.post(`/${resource}`, payload).then((r) => r.data),
  update: (id, payload) => api.put(`/${resource}/${id}`, payload).then((r) => r.data),
  remove: (id) => api.delete(`/${resource}/${id}`).then((r) => r.data),
});

export const dashboardApi = { get: () => api.get("/dashboard").then((r) => r.data) };
export const websiteApi = {
  home: () => api.get("/website/home").then((r) => r.data),
  about: () => api.get("/website/about").then((r) => r.data),
};
export const membersApi = createCrudApi("members");
export const trainersApi = createCrudApi("trainers");
export const plansApi = createCrudApi("membershipplans");
export const paymentsApi = createCrudApi("payments");
export const equipmentApi = createCrudApi("equipment");
export const workoutProgramsApi = createCrudApi("workoutprograms");
export const memberProgramsApi = createCrudApi("memberprograms");
export const reportsApi = {
  paymentsByMember: () => api.get("/reports/payments-by-member").then((r) => r.data),
  equipmentByStatus: () => api.get("/reports/equipment-by-status").then((r) => r.data),
  programEnrollment: () => api.get("/reports/program-enrollment").then((r) => r.data),
};
export default api;
