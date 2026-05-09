import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Student } from '../../../core/models/student.model';
import { StudentService } from '../../../core/services/student.service';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './student-list.html',
  styleUrl: './student-list.scss'
})
export class StudentListComponent implements OnInit {
  private readonly studentService = inject(StudentService);
  private readonly toastr = inject(ToastrService);

  protected readonly students = signal<Student[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly pageNumber = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly sortBy = signal<string>('createddate');
  protected readonly sortDescending = signal(true);
  protected readonly loading = signal(false);

  protected readonly searchControl = new FormControl<string>('', { nonNullable: true });

  protected readonly pages = (): number[] => {
    const total = Math.max(1, Math.ceil(this.totalCount() / this.pageSize()));
    const current = this.pageNumber();
    const window = 2;
    const start = Math.max(1, current - window);
    const end = Math.min(total, current + window);
    const result: number[] = [];
    for (let i = start; i <= end; i++) result.push(i);
    return result;
  };

  protected readonly totalPages = (): number => Math.max(1, Math.ceil(this.totalCount() / this.pageSize()));

  ngOnInit(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.pageNumber.set(1);
        this.fetch();
      });

    this.fetch();
  }

  protected sortOn(column: string): void {
    if (this.sortBy() === column) {
      this.sortDescending.update(v => !v);
    } else {
      this.sortBy.set(column);
      this.sortDescending.set(false);
    }
    this.fetch();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.pageNumber()) return;
    this.pageNumber.set(page);
    this.fetch();
  }

  protected setPageSize(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.fetch();
  }

  protected delete(student: Student): void {
    const confirmed = confirm(`Delete '${student.name}'? This cannot be undone.`);
    if (!confirmed) return;

    this.studentService.delete(student.id).subscribe(response => {
      if (response.success) {
        this.toastr.success(response.message);
        this.fetch();
      }
    });
  }

  private fetch(): void {
    this.loading.set(true);
    this.studentService.list({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      search: this.searchControl.value || undefined,
      sortBy: this.sortBy(),
      sortDescending: this.sortDescending()
    }).subscribe({
      next: response => {
        this.loading.set(false);
        if (response.success) {
          this.students.set(response.data.items);
          this.totalCount.set(response.data.totalCount);
        }
      },
      error: () => this.loading.set(false)
    });
  }
}
