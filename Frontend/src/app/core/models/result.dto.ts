export interface ResultDto<T> {
  success: boolean;
  result?: T;
  error?: string;
  statusCode: number;
}
