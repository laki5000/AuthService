import { RoleDto } from "./role.dto";

export interface UpdateUserRoleDto extends RoleDto {
  Username: string;
  Add: boolean;
}
