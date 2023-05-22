<?php

namespace App\Http\Controllers\Admin;

use App\Http\Requests\AppointmentRequest;
use Backpack\CRUD\app\Http\Controllers\CrudController;
use Backpack\CRUD\app\Library\CrudPanel\CrudPanelFacade as CRUD;

/**
 * Class AppointmentCrudController
 * @package App\Http\Controllers\Admin
 * @property-read \Backpack\CRUD\app\Library\CrudPanel\CrudPanel $crud
 */
class AppointmentCrudController extends CrudController
{
    use \Backpack\CRUD\app\Http\Controllers\Operations\ListOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\CreateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\UpdateOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\DeleteOperation;
    use \Backpack\CRUD\app\Http\Controllers\Operations\ShowOperation;

    /**
     * Configure the CrudPanel object. Apply settings to all operations.
     *
     * @return void
     */
    public function setup()
    {
        CRUD::setModel(\App\Models\Appointment::class);
        CRUD::setRoute(config('backpack.base.route_prefix') . '/appointment');
        CRUD::setEntityNameStrings('appointment', 'appointments');
    }

    /**
     * Define what happens when the List operation is loaded.
     *
     * @see  https://backpackforlaravel.com/docs/crud-operation-list-entries
     * @return void
     */
    protected function setupListOperation()
    {
        // CRUD::column('user_id');
        CRUD::column('user_id')
            ->type('relationship')
            ->attribute('user.staff_id')
            ->label('User')
            ->wrapper([
                'href' => function ($crud, $column, $entry, $related_key) {
                    $user = \App\Models\User::find($related_key);
                    if ($user) {
                        return backpack_url('user/' . $user->id);
                    }

                    return backpack_url('user/' . $related_key);
                    // return '#';
                },
                'target' => '_blank',
            ])
            ->value(function ($entry) {
                return $entry->user->staff_id ?? '-';
            });

        CRUD::column('start_dt');
        CRUD::column('customer_ID');
        CRUD::column('customer_phone');
        CRUD::column('customer_address');
        CRUD::column('latitude');
        CRUD::column('longitude');
        CRUD::column('is_complete');
        CRUD::column('created_at');
        CRUD::column('updated_at');

        /**
         * Columns can be defined using the fluent syntax or array syntax:
         * - CRUD::column('price')->type('number');
         * - CRUD::addColumn(['name' => 'price', 'type' => 'number']);
         */
    }

    /**
     * Define what happens when the Create operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-create
     * @return void
     */
    protected function setupCreateOperation()
    {
        CRUD::setValidation(AppointmentRequest::class);

        CRUD::field('user_id')
            ->attribute('staff_id')
            ->type('select')
            ->label('User');

        // CRUD::field('user_id');
        CRUD::field('start_dt');
        CRUD::field('customer_ID');
        CRUD::field('customer_phone');
        CRUD::field('customer_address');
        CRUD::field('latitude');
        CRUD::field('longitude');
        CRUD::field('is_complete');

        /**
         * Fields can be defined using the fluent syntax or array syntax:
         * - CRUD::field('price')->type('number');
         * - CRUD::addField(['name' => 'price', 'type' => 'number']));
         */
    }

    /**
     * Define what happens when the Update operation is loaded.
     *
     * @see https://backpackforlaravel.com/docs/crud-operation-update
     * @return void
     */
    protected function setupUpdateOperation()
    {
        $this->setupCreateOperation();
    }
}