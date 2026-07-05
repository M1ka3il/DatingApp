import { Component, OnInit, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MemberService } from '../../core/services/member';
import { Member } from '../../core/models/member';

@Component({
  selector: 'app-member-detail',
  imports: [RouterLink],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.css',
})
export class MemberDetail implements OnInit {
  private memberService = inject(MemberService);
  id = input.required<string>();
  protected member = signal<Member | undefined>(undefined);

  ngOnInit() {
    this.memberService.getMember(this.id()).subscribe({
      next: (member) => this.member.set(member),
    });
  }
}
