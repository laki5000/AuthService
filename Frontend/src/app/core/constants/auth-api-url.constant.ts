import { environment } from '../../../environments/environment';

const baseUrl = `${environment.apiBaseUrl}/api/Auth`;

export const AuthApiUrlConstant = {
  login: `${baseUrl}/login`,
  register: `${baseUrl}/register`,
  logout: `${baseUrl}/logout`,
  checkAuth: `${baseUrl}/checkAuth`,
  amIAdmin: `${baseUrl}/amIAdmin`,
  updateUserRole: `${baseUrl}/updateUserRole`,
  getAllRoles: `${baseUrl}/getAllRoles`,
  createRole: `${baseUrl}/createRole`,
};
