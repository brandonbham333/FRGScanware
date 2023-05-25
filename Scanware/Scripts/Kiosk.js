/*  **********************************************************
 *  BuildOptionList -   Create list of states or cities for
 *                      Destination selection
 *                      
 *  element - DOM reference: parent to append child results to
 *  value   - string: selected state value, if creating city
 *  ********************************************************** */
async function BuildOptionList(element, value = null) {
    // get node children
    const range = document.createRange();
    range.selectNodeContents(element);

    // retain "Select <Location>" option, delete all others
    if (element.length > 1) {
        range.setStartAfter(element.children[0]);
        range.deleteContents();
    }
    
    const url = value === null ? '/Kiosk/States' : `/Kiosk/Cities?state=${value}`

    let fragment = new DocumentFragment();
    // populate options list
    fetch(url)
        .then(resp => resp.json())
        .then(result => result.forEach(
            loc => {
                let opt = document.createElement('option');
                opt.value = value === null ? loc['state_abbr'] : loc;
                opt.innerText = value === null ? `${loc['state_abbr']} | ${loc['state_long']}` : loc;
                fragment.appendChild(opt);
            }))
        .catch(e => console.log(`Error: ${e}`))
        .finally(() => element.appendChild(fragment));
};
