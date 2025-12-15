import { environment } from '../../../environments/environment';

const baseUrl = `${environment.apiBaseUrl}/api/Role`;

export const RoleApiUrlConstant = {
  getAll: `${baseUrl}`,
  create: `${baseUrl}`,
};
