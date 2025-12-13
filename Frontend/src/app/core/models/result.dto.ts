export interface ResultDto<T> {
  Success: boolean;
  Result?: T;
  Error?: string;
  StatusCode: number;
}
