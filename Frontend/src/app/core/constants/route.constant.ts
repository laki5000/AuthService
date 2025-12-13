export const RouteConstants = {
  LOGIN: 'login',
  REGISTER: 'register',
  DASHBOARD: 'dashboard',

  get LOGIN_PATH() {
    return `/${this.LOGIN}`;
  },
  get REGISTER_PATH() {
    return `/${this.REGISTER}`;
  },
  get DASHBOARD_PATH() {
    return `/${this.DASHBOARD}`;
  },
};
