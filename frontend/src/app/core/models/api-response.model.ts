export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
  statusCode: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}
