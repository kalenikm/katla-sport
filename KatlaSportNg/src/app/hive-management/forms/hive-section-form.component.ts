import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveSectionService } from '../services/hive-section.service';
import { HiveService } from '../services/hive.service';
import { HiveSection } from '../models/hive-section';
import { HiveListItem } from '../models/hive-list-item';

@Component({
  selector: 'app-hive-section-form',
  templateUrl: './hive-section-form.component.html',
  styleUrls: ['./hive-section-form.component.css']
})
export class HiveSectionFormComponent implements OnInit {

  hiveSection = new HiveSection(0, "", "", false, "", 1);
  existed = false;
  hiveSectionId: number;
  _hiveId: number;
  hives: HiveListItem[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveSectionService: HiveSectionService,
    private hiveService: HiveService
  ) { }

  ngOnInit() {
    this.hiveService.getHives().subscribe(h => this.hives = h);

    this.route.params.subscribe(p => {
      if (p['hiveId'] !== undefined) {
        this._hiveId = parseInt(p['hiveId']);
        this.hiveService.getHive(this._hiveId).subscribe(h => this.hiveSection.hiveId = this._hiveId);
        this.existed = false;
        return;
      }});

    this.route.params.subscribe(p => {
      if (p['id'] !== undefined) {
      this.hiveSectionId = p['id'];
      this.hiveSectionService.getHiveSection(this.hiveSectionId).subscribe(h => {this.hiveSection = h; this._hiveId = h.hiveId;});
      this._hiveId = this.hiveSection.hiveId;
      this.existed = true;
      return;
    }});
  }

  navigateToSections() {
    this.router.navigate([`/hive/${this._hiveId}/sections`]);
  }

  onCancel() {
    this.navigateToSections();
  }
  
  onSubmit() {
    this._hiveId = this.hiveSection.hiveId;
    if (this.existed) {
      this.hiveSection.id = this.hiveSectionId;
      this.hiveSectionService.updateHiveSection(this.hiveSection).subscribe(h => this.navigateToSections());
    } else {
      this.hiveSectionService.addHiveSection(this.hiveSection).subscribe(h => this.navigateToSections());
    }
  }

  onDelete() {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSectionId, true).subscribe(h => this.hiveSection.isDeleted = true);
  }

  onUndelete() {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSectionId, false).subscribe(h => this.hiveSection.isDeleted = false);
  }

  onPurge() {
    if (this.hiveSection.isDeleted) {
      this.hiveSectionService.deleteHiveSection(this.hiveSectionId).subscribe(h => this.navigateToSections());
    }
  }
}
