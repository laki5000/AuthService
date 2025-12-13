import { environment } from "../../../environments/environment";

const baseUrl = `${environment.apiBaseUrl}/api/User`;

export const UserApiUrlConstant = {
  login: `${baseUrl}/login`,
  register: `${baseUrl}/register`,
  logout: `${baseUrl}/logout`,
  checkAuth: `${baseUrl}/checkAuth`,
};