import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { CreateStudentPayload, Student, StudentQuery, UpdateStudentPayload } from '../models/student.model';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/students`;

  list(query: StudentQuery): Observable<ApiResponse<PagedResult<Student>>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.search) params = params.set('search', query.search);
    if (query.course) params = params.set('course', query.course);
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDescending !== undefined) params = params.set('sortDescending', query.sortDescending);

    return this.http.get<ApiResponse<PagedResult<Student>>>(this.base, { params });
  }

  getById(id: number): Observable<ApiResponse<Student>> {
    return this.http.get<ApiResponse<Student>>(`${this.base}/${id}`);
  }

  create(payload: CreateStudentPayload): Observable<ApiResponse<Student>> {
    return this.http.post<ApiResponse<Student>>(this.base, payload);
  }

  update(payload: UpdateStudentPayload): Observable<ApiResponse<Student>> {
    return this.http.put<ApiResponse<Student>>(`${this.base}/${payload.id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
