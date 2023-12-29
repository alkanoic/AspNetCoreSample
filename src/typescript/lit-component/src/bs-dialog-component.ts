import { Modal } from 'bootstrap';
import { LitElement, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';

@customElement('bs-dialog-component')
export class BsDialogComponent extends LitElement {
  protected createRenderRoot() {
    return this;
  }

  @property()
  dialogId?: string;

  private dialog?: Modal;

  protected updated() {
    const dialogElement = this.querySelector(`#${this.dialogId}`);
    
    if (dialogElement) {
      this.dialog = new Modal(dialogElement);
    } else {
      console.error(`Dialog with id ${this.dialogId} not found.`);
    }
  }

  public close(e: Event) {
    const detail = {success: Boolean}
    const event = new CustomEvent('saveChanges', { detail, cancelable: true})
    this.dispatchEvent(event);
    if (event.defaultPrevented) {
      e.preventDefault();
    }
    if(!detail.success){
      return;
    }
    if (this.dialog) {
      this.dialog.hide();
    }
  }

  render() {
    return html`
      <div
        class="modal fade"
        id="${this.dialogId}"
        tabindex="-1"
        aria-labelledby="${this.dialogId}-label"
        aria-hidden="true"
      >
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title" id="${this.dialogId}-label">Dialog Title</h5>
              <button
                type="button"
                class="btn-close"
                data-bs-dismiss="modal"
                aria-label="Close"
              ></button>
            </div>
            <div class="modal-body">
              <p>Dialog content goes here.</p>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
              <button type="button" class="btn btn-primary" @click="${this.close}">
                Save changes
              </button>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}
