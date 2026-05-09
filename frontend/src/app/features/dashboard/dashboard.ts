import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StudentService } from '../../core/services/student.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  private readonly studentService = inject(StudentService);

  protected readonly totalStudents = signal<number | null>(null);
  protected readonly activeCourses = signal<number | null>(null);
  protected readonly recent = signal<{ name: string; course: string; createdDate: string }[]>([]);

  ngOnInit(): void {
    this.loadStats();
  }

  private loadStats(): void {
    this.studentService.list({ pageNumber: 1, pageSize: 5, sortBy: 'createddate', sortDescending: true })
      .subscribe(response => {
        if (!response.success) return;
        this.totalStudents.set(response.data.totalCount);
        this.recent.set(response.data.items.map(s => ({ name: s.name, course: s.course, createdDate: s.createdDate })));
        const courses = new Set(response.data.items.map(s => s.course));
        this.activeCourses.set(courses.size);
      });
  }
}
