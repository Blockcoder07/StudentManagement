export interface Student {
  id: number;
  name: string;
  email: string;
  age: number;
  course: string;
  createdDate: string;
  modifiedDate?: string | null;
  isActive: boolean;
}

export interface CreateStudentPayload {
  name: string;
  email: string;
  age: number;
  course: string;
}

export interface UpdateStudentPayload extends CreateStudentPayload {
  id: number;
  isActive: boolean;
}

export interface StudentQuery {
  pageNumber: number;
  pageSize: number;
  search?: string;
  course?: string;
  sortBy?: string;
  sortDescending?: boolean;
}
