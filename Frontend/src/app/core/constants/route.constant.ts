export const RouteConstants = {
  Login: 'login',
  Register: 'register',
  Dashboard: 'dashboard',

  get LoginPath() {
    return `/${this.Login}`;
  },
  get RegisterPath() {
    return `/${this.Register}`;
  },
  get DashboardPath() {
    return `/${this.Dashboard}`;
  },
};
