import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CreateStudentPayload, UpdateStudentPayload } from '../../../core/models/student.model';
import { StudentService } from '../../../core/services/student.service';

@Component({
  selector: 'app-student-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './student-form.html',
  styleUrl: './student-form.scss'
})
export class StudentFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  protected readonly studentId = signal<number | null>(null);
  protected readonly submitting = signal(false);
  protected readonly isEditMode = computed(() => this.studentId() !== null);
  protected readonly title = computed(() => (this.isEditMode() ? 'Edit Student' : 'Add Student'));

  protected readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z\s.'-]+$/)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
    age: [18, [Validators.required, Validators.min(5), Validators.max(120)]],
    course: ['', [Validators.required, Validators.maxLength(100)]],
    isActive: [true]
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    if (Number.isNaN(id)) {
      this.router.navigate(['/students']);
      return;
    }

    this.studentId.set(id);
    this.studentService.getById(id).subscribe(response => {
      if (response.success) {
        this.form.patchValue({
          name: response.data.name,
          email: response.data.email,
          age: response.data.age,
          course: response.data.course,
          isActive: response.data.isActive
        });
      }
    });
  }

  protected submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const formValue = this.form.getRawValue();

    if (this.isEditMode()) {
      const payload: UpdateStudentPayload = { id: this.studentId()!, ...formValue };
      this.studentService.update(payload).subscribe({
        next: response => {
          this.submitting.set(false);
          if (response.success) {
            this.toastr.success(response.message);
            this.router.navigate(['/students']);
          }
        },
        error: () => this.submitting.set(false)
      });
    } else {
      const payload: CreateStudentPayload = {
        name: formValue.name,
        email: formValue.email,
        age: formValue.age,
        course: formValue.course
      };
      this.studentService.create(payload).subscribe({
        next: response => {
          this.submitting.set(false);
          if (response.success) {
            this.toastr.success(response.message);
            this.router.navigate(['/students']);
          }
        },
        error: () => this.submitting.set(false)
      });
    }
  }
}
