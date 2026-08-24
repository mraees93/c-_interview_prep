import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, computed, effect, ElementRef, inject, OnDestroy, OnInit, output, Signal, signal, viewChild, WritableSignal } from '@angular/core';
import { LegalCase } from '../../models/legal-case.model';

@Component({
    selector: 'app-case-search',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './case-search.component.html',
    styleUrl: './case-search.component.css'
})

export class CaseSearchComponent implements OnInit, OnDestroy {
    private http = inject(HttpClient);
    // #region CONCEPT 4: PROP CALL BACKS (React Props vs. Angular Output)
    // REACT TRANSLATION: Behaves exactly like a parent callback prop: onCaseSelected={handleSelect}
    // #endregion
    onCaseSelected = output<LegalCase>();

    // #region CONCEPT 2: STATE HOOKS (useState vs. signal)
    // REACT TRANSLATION: signal() replaces useState(). State is read via cases() and updated via cases.set().
    // #endregion
    cases: WritableSignal<LegalCase[]> = signal([]);
    searchQuery: WritableSignal<string> = signal("");
    isLoading: WritableSignal<boolean> = signal(false);

    // #region CONCEPT 5 (PART 2): MUTABLE CACHING (useRef Value vs. Class Property)
    // REACT TRANSLATION: Behaves exactly like: const accessCount = useRef(0)
    // Modifying this variable stores local tracking data but completely bypasses UI re-renders.
    // #endregion
    diagnosticAccessCount: number = 0;

    // #region CONCEPT 5 (PART 1): DOM REFS (useRef DOM vs. viewChild Signal Query)
    // REACT TRANSLATION: Behaves exactly like: const searchInputRef = useRef<HTMLInputElement>(null)
    // #endregion
    searchInputRef = viewChild<ElementRef<HTMLInputElement>>('searchInput');

    // #region CONCEPT 6: CALCULATION BUFFERS (useMemo vs. computed)
    // REACT TRANSLATION: Behaves exactly like: useMemo(() => filterLogic, [cases, searchQuery, courtFilter])
    // Caches output arrays natively until underlying signal dependencies change.
    // #endregion
    filteredCases: Signal<LegalCase[]> = computed(() => {
        const activeQuery = this.searchQuery().toLowerCase();

        this.diagnosticAccessCount++;

        return this.cases().filter(caseItem => 
                caseItem.title.toLowerCase().includes(activeQuery) || caseItem.caseNumber.toLowerCase().includes(activeQuery)
        );
    });

    totalMatchesCount: Signal<number> = computed(() => this.filteredCases().length);

    constructor() {
      // #region CONCEPT 3 (PART 2): RUNTIME SIDE EFFECTS (useEffect Triggers vs. effect)
      // REACT TRANSLATION: Behaves exactly like: useEffect(() => { log() }, [searchQuery])
      // Auto-detects dependencies referenced inside the closure block.
      // #endregion  
      effect(() => {
        console.log(`Telemetry system change query alert: ${this.searchQuery()}`);
      })
    }

    // #region CONCEPT 3 (PART 1): ON-MOUNT LIFECYCLES (useEffect [] vs. ngOnInit)
    // REACT TRANSLATION: Behaves exactly like an empty dependency hook: useEffect(() => { fetch() }, [])
    // #endregion
    ngOnInit(): void {
        this.fetchLegalRecords();
    }

    // #region CLEANUP OPERATIONS (useEffect Return Cleanup vs. ngOnDestroy)
    // REACT TRANSLATION: Behaves exactly like the cleanup return statement: return () => { cleanup() }
    // #endregion
    ngOnDestroy(): void {
        console.log('Tearing down active dashboard socket streams. Zero memory leaks allowed.');
    }

    private fetchLegalRecords(): void {
        this.isLoading.set(true);
        this.http.get<LegalCase[]>('https://lexisnexis-mock.capetown')
            .subscribe({
                next: (data) => {
                    this.cases.set(data);
                    this.isLoading.set(false);
                },
                error: (err) => {
                    console.error('API consumption failure stream detected:', err);
                    this.isLoading.set(false);   
                }
            });
    }

    onSearchChange(value: string): void {
        this.searchQuery.set(value);
    }

    selectCase(selectedRecord: LegalCase): void {
        this.onCaseSelected.emit(selectedRecord);
    }

    focusSearchInput(): void {
        const element = this.searchInputRef()?.nativeElement;
        if(element) {
            element.focus();
        }
    }
}